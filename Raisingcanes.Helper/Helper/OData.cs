using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Raisingcanes.Model;
using System.Net;
using System.Xml.Linq;
using System.Configuration;
using System.Xml;
using System.Runtime.Serialization.Formatters.Binary;
using Newtonsoft.Json;

namespace Raisingcanes.Helper
{
    public class OData : IDisposable
    {
        CookieContainer AuthCookie = new CookieContainer();
        private static readonly XNamespace ds = "http://schemas.microsoft.com/ado/2007/08/dataservices";
        private static readonly XNamespace dsmd = "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata";
        private static readonly XNamespace atom = "http://www.w3.org/2005/Atom";

        string baseUri;
        string authServiceUri;
        string serverUriUsr;

        public OData()
        {

            baseUri = ConfigurationManager.AppSettings["ServerUriUsr"].ToString();
            authServiceUri = baseUri + @"/ServiceModel/AuthService.svc/Login";
            serverUriUsr = baseUri + @"/0/ServiceModel/EntityDataService.svc/";

            GetConnectionBPM();
        }
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Dispose(true);
        }
        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                CookieCollection cookieCollection = AuthCookie.GetCookies(new Uri(authServiceUri));
                WriteCookiesToDisk(AuthCookie);
            }

        }

        #region Connection
        private void GetConnectionBPM()
        {


            string file = System.IO.Path.Combine(ConfigurationManager.AppSettings["PathCookie"].ToString(), "cookies.dat");
            AuthCookie = ReadCookiesFromDisk(file);
            LoginBPM();


        }

        private bool LoginBPM()
        {
            string userName = ConfigurationManager.AppSettings["UserOData"].ToString();
            string userPassword = ConfigurationManager.AppSettings["PassOData"].ToString();
            CookieCollection cookieCollection = AuthCookie.GetCookies(new Uri(authServiceUri));
            string csrfToken = cookieCollection["BPMCSRF"].Value;

            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                   | SecurityProtocolType.Tls11
                   | SecurityProtocolType.Tls12
                   | SecurityProtocolType.Ssl3;

            var authRequest = HttpWebRequest.Create(authServiceUri) as HttpWebRequest;

            authRequest.Method = "POST";
            authRequest.ContentType = "application/json";
            authRequest.CookieContainer = AuthCookie;
            //authRequest.Headers.Set("ForceUseSession", "true");
            //authRequest.Headers.Add("BPMCSRF", csrfToken);

            using (var requestStream = authRequest.GetRequestStream())
            {
                using (var writer = new StreamWriter(requestStream))
                {
                    writer.Write(@"{
                                ""UserName"":""" + userName + @""",
                                ""UserPassword"":""" + userPassword + @""",
                                ""SolutionName"":""TSBpm"",
                                ""TimeZoneOffset"":-120,
                                ""Language"":""En-us""
                                }");
                }
            }



            ResponseStatus status = null;
            using (var response = (HttpWebResponse)authRequest.GetResponse())
            {
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    string responseText = reader.ReadToEnd();
                    status = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<ResponseStatus>(responseText);
                }

            }

            if (status != null)
            {
                if (status.Code == 0)
                {
                    //CookieCollection cookieCollection = AuthCookie.GetCookies(new Uri(authServiceUri));
                    WriteCookiesToDisk(AuthCookie);
                    return true;

                }
                return true;

            }


            return false;
        }

        private void WriteCookiesToDisk(CookieContainer cookieJar)
        {
            string file = System.IO.Path.Combine(ConfigurationManager.AppSettings["PathCookie"].ToString(), "cookies.dat");

            using (Stream stream = File.Create(file))
            {
                try
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(stream, cookieJar);
                }
                catch
                {

                }
            }
        }

        private CookieContainer ReadCookiesFromDisk(string file)
        {

            try
            {
                using (Stream stream = File.Open(file, FileMode.Open))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    return (CookieContainer)formatter.Deserialize(stream);
                }
            }
            catch
            {
                return new CookieContainer();
            }
        }

        #endregion

        #region Common

        public bool SaveData(Data _data)
        {
            bool result = false;

            try
            {

                #region City

                _data.oAccount.oAddress.CityId = FindCityByNameAndRegionId(_data.oAccount.oAddress.City, _data.oAccount.oAddress.StateId);
                _data.oContact.oAddress.CityId = _data.oAccount.oAddress.CityId;
                if (string.IsNullOrEmpty(_data.oAccount.oAddress.CityId))
                {
                    _data.oAccount.oAddress.CityId = CreateCity(_data.oAccount.oAddress.City, _data.oAccount.oAddress.StateId);
                    _data.oContact.oAddress.CityId = _data.oAccount.oAddress.CityId;
                }

                if (_data.oCase.AnotherAddress)
                {
                    _data.oCase.oAddress.CityId = FindCityByNameAndRegionId(_data.oCase.oAddress.City, _data.oCase.oAddress.StateId);
                    if (string.IsNullOrEmpty(_data.oCase.oAddress.CityId))
                    {
                        _data.oCase.oAddress.CityId = CreateCity(_data.oCase.oAddress.City, _data.oCase.oAddress.StateId);
                    }
                }


                #endregion

                #region Account
                bool _newAccount = false;
                List<LookupItem> listAcc = new List<LookupItem>();

                listAcc = GetEntityIdByFilters(_data.oAccount.Name, _data.oAccount.PhoneNumber, _data.oAccount.oAddress.Street, _data.oAccount.oAddress.City, _data.oAccount.oAddress.StateId, _data.oAccount.oAddress.Zip);

                if (listAcc.Count == 0 || listAcc.Count > 1)
                {
                    _data.oAccount.Id = CreateAccount(_data.oAccount);
                    _newAccount = true;
                }
                else
                {
                    /*Fix 02/20/2020 - Eric Garcia de Campos*/
                    _data.oAccount.Id = listAcc[0].Id.ToString();
                    UpdateAccount(_data.oAccount);
                }
                #endregion

                #region Contact

                _data.oContact.Id = GetEntityIdByField("Contact", "Email", _data.oContact.Email);
                if (string.IsNullOrEmpty(_data.oContact.Id))
                {
                    _data.oContact.AccountId = _data.oAccount.Id;
                    _data.oContact.Id = CreateContact(_data.oContact);
                }
                else
                {
                    string accId = GetEntityFieldByID("Contact", "AccountId", _data.oContact.Id);
                    if (string.IsNullOrEmpty(accId))
                        _data.oContact.AccountId = _data.oAccount.Id;

                    UpdateConctact(_data.oContact);
                }
                #endregion

                #region Contact - preferred restaurants

                if (!string.IsNullOrEmpty(_data.oContact.PreferredRestaurant))
                {
                    if (!ExistsPreferredRestaurantContact(_data.oContact.Id, _data.oContact.PreferredRestaurant))
                    {
                        CreatePreferredRestaurantContact(_data.oContact.Id, _data.oContact.PreferredRestaurant);
                    }
                }


                #endregion

                #region Primary Contact

                if (_newAccount)
                {
                    UpdateEntityFieldByID("Account", "PrimaryContactId", _data.oAccount.Id, _data.oContact.Id);
                }

                #endregion

                #region Relationship account contact

                if (!ExistsOrganizationContact(_data.oAccount.Id, _data.oContact.Id))
                {
                    CreateOrganizationContact(_data.oAccount.Id, _data.oContact.Id);
                }

                #endregion

                #region Case

                _data.oCase.AccountId = _data.oAccount.Id;
                _data.oCase.ContactId = _data.oContact.Id;
                _data.oCase.Id = CreateCase(_data.oCase, _data.oAccount);
                if (!string.IsNullOrEmpty(_data.oCase.Id))
                {
                    if (_data.oCase.FileAdditional != null)
                    {
                        _data.oCase.FileAdditional.EntityId = _data.oCase.Id;
                        CreateCaseFile(_data.oCase.FileAdditional);
                    }

                    if (_data.oCase.FileW9 != null)
                    {
                        _data.oCase.FileW9.EntityId = _data.oCase.Id;
                        CreateCaseFile(_data.oCase.FileW9);
                    }
                }
                result = true;
                #endregion

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return result;
        }

        public List<LookupItem> GetLookup(string table, string field)
        {

            try
            {
                string requestUri = serverUriUsr + table + "Collection?$top=1000&$select=Id," + field + "&$orderby=" + field;

                var request = HttpWebRequest.Create(requestUri) as HttpWebRequest;
                request.Method = "GET";
                CookieCollection cookieCollection = AuthCookie.GetCookies(new Uri(authServiceUri));
                string csrfToken = cookieCollection["BPMCSRF"].Value;
                request.CookieContainer = AuthCookie;
                request.Headers.Add("BPMCSRF", csrfToken);
                request.Headers.Set("ForceUseSession", "true");

                using (var response = request.GetResponse())
                {
                    if (((HttpWebResponse)response).StatusCode == HttpStatusCode.OK)
                    {
                        XDocument xmlDoc = XDocument.Load(response.GetResponseStream());
                        response.Close();


                        IEnumerable<LookupItem> items = from entry in xmlDoc.Descendants(dsmd + "properties")
                                                        select new LookupItem()
                                                        {
                                                            Id = entry.Element(ds + "Id").Value,
                                                            Text = entry.Element(ds + field).Value
                                                        };


                        return items.ToList();
                    }
                }
            }
            catch
            {
            }

            return new List<LookupItem>();

        }
        public string GetEntityIdByField(string table, string field, string value)
        {
            try
            {
                string requestUri = serverUriUsr + table + "Collection?$filter = " + field + " eq '" + value + "'";
                var request = HttpWebRequest.Create(requestUri) as HttpWebRequest;
                request.Method = "GET";
                CookieCollection cookieCollection = AuthCookie.GetCookies(new Uri(authServiceUri));
                string csrfToken = cookieCollection["BPMCSRF"].Value;
                request.CookieContainer = AuthCookie;
                request.Headers.Add("BPMCSRF", csrfToken);
                request.Headers.Set("ForceUseSession", "true");
                using (var response = request.GetResponse())
                {
                    if (((HttpWebResponse)response).StatusCode == HttpStatusCode.OK)
                    {
                        XDocument xmlDoc = XDocument.Load(response.GetResponseStream());
                        response.Close();

                        var items = from entry in xmlDoc.Descendants(ds + "Id")
                                    select entry.Value;

                        return items.FirstOrDefault();
                    }
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                throw (ex);
            }

        }

        //Custom Querys get Validation View Account
        public List<LookupItem> GetEntityIdByFilters(string name, string phone, string address, string city, string state, string zip)
        {
            string requestUri = serverUriUsr + "UsrVwAccountAndAccountAddressCollection?$filter = UsrOrganizationName eq '" + name + "' and UsrOrganizationPhone eq '" + phone + "' and UsrOrganizationAddress eq '" + address + "' and UsrOrganizationCity eq '" + city + "' and UsrOrganizationStateId eq '" + state + "' and UsrOrganizationZip eq '" + zip + "' ";

            var request = HttpWebRequest.Create(requestUri) as HttpWebRequest;
            request.Method = "GET";
            CookieCollection cookieCollection = AuthCookie.GetCookies(new Uri(authServiceUri));
            string csrfToken = cookieCollection["BPMCSRF"].Value;
            request.CookieContainer = AuthCookie;
            request.Headers.Add("BPMCSRF", csrfToken);
            request.Headers.Set("ForceUseSession", "true");

            using (var response = request.GetResponse())
            {
                if (((HttpWebResponse)response).StatusCode == HttpStatusCode.OK)
                {
                    XDocument xmlDoc = XDocument.Load(response.GetResponseStream());
                    response.Close();


                    IEnumerable<LookupItem> items = from entry in xmlDoc.Descendants(dsmd + "properties")
                                                    select new LookupItem()
                                                    {
                                                        Id = entry.Element(ds + "UsrOrganizationId").Value
                                                    };
                    items = items.GroupBy(i => i.Id).Select(grp => grp.First()).ToList();
                    return items.ToList();
                }
                else
                {
                    return null;
                }
            }
        }

        //Custom Querys get City
        public string GetEntityIdByFieldCustom(string table, string field, string value)
        {
            string requestUri = serverUriUsr + table + "Collection?$filter = " + field + " eq guid'" + value + "'";

            var request = HttpWebRequest.Create(requestUri) as HttpWebRequest;
            request.Method = "GET";
            CookieCollection cookieCollection = AuthCookie.GetCookies(new Uri(authServiceUri));
            string csrfToken = cookieCollection["BPMCSRF"].Value;
            request.CookieContainer = AuthCookie;
            request.Headers.Add("BPMCSRF", csrfToken);
            request.Headers.Set("ForceUseSession", "true");

            using (var response = request.GetResponse())
            {
                if (((HttpWebResponse)response).StatusCode == HttpStatusCode.OK)
                {
                    XDocument xmlDoc = XDocument.Load(response.GetResponseStream());
                    response.Close();

                    var items = from entry in xmlDoc.Descendants(ds + "Name")
                                select entry.Value;

                    return items.FirstOrDefault();

                    //items = items.GroupBy(i => i.Id).Select(grp => grp.First()).ToList();
                    //items = items.Where(x => x.Text != "").ToList(); 
                    //items = items.OrderBy(x => x.Text);
                    //return JsonConvert.SerializeObject(items.ToList());
                }
                else
                {
                    return "";
                }
            }
        }

        //Custom Querys get Restaurants
        public string GetRestaurants(string table, string field, string value)
        {
            string requestUri = serverUriUsr + table + "Collection?$filter = " + field + " eq guid'" + value + "'";

            var request = HttpWebRequest.Create(requestUri) as HttpWebRequest;
            request.Method = "GET";
            CookieCollection cookieCollection = AuthCookie.GetCookies(new Uri(authServiceUri));
            string csrfToken = cookieCollection["BPMCSRF"].Value;
            request.CookieContainer = AuthCookie;
            request.Headers.Add("BPMCSRF", csrfToken);
            request.Headers.Set("ForceUseSession", "true");

            using (var response = request.GetResponse())
            {
                if (((HttpWebResponse)response).StatusCode == HttpStatusCode.OK)
                {
                    XDocument xmlDoc = XDocument.Load(response.GetResponseStream());
                    response.Close();


                    IEnumerable<Address> items = from entry in xmlDoc.Descendants(dsmd + "properties")
                                                 select new Address()
                                                 {
                                                     Street = entry.Element(ds + "UsrAddress").Value,
                                                     Zip = entry.Element(ds + "UsrZipCode").Value,
                                                     RestaurantCode = entry.Element(ds + "Id").Value,
                                                     CityId = this.GetEntityIdByFieldCustom("City", "Id", entry.Element(ds + "UsrCityId").Value),
                                                     StateId = this.GetEntityIdByFieldCustom("Region", "Id", entry.Element(ds + "UsrStateId").Value)
                                                 };
                    return JsonConvert.SerializeObject(items.ToList());
                }
                else
                {
                    return "";
                }
            }
        }

        private string GetEntityFieldByID(string table, string field, string Id)
        {
            try
            {
                string requestUri = serverUriUsr + table + "Collection(guid'" + Id + "')/" + field;

                var request = HttpWebRequest.Create(requestUri) as HttpWebRequest;
                request.Method = "GET";
                CookieCollection cookieCollection = AuthCookie.GetCookies(new Uri(authServiceUri));
                string csrfToken = cookieCollection["BPMCSRF"].Value;
                request.CookieContainer = AuthCookie;
                request.Headers.Add("BPMCSRF", csrfToken);
                request.Headers.Set("ForceUseSession", "true");

                using (var response = request.GetResponse())
                {
                    if (((HttpWebResponse)response).StatusCode == HttpStatusCode.OK)
                    {
                        XDocument xmlDoc = XDocument.Load(response.GetResponseStream());
                        response.Close();

                        var items = from entry in xmlDoc.Descendants(ds + field)
                                    select entry.Value;

                        return items.FirstOrDefault();
                    }
                }

            }
            catch
            {
            }
            return string.Empty;
        }
        private bool UpdateEntityFieldByID<T>(string table, string field, string Id, T value)
        {
            try
            {
                var content = new XElement(dsmd + "properties",
                            new XElement(ds + field, value));


                var entry = new XElement(atom + "entry",
                    new XElement(atom + "content",
                        new XAttribute("type", "application/xml"),
                            content)
                    );

                var request = (HttpWebRequest)HttpWebRequest.Create(serverUriUsr + table + "Collection(guid'" + Id + "')");
                request.Method = "PUT";
                request.Accept = "application/atom+xml";
                request.ContentType = "application/atom+xml;type=entry";
                CookieCollection cookieCollection = AuthCookie.GetCookies(new Uri(authServiceUri));
                string csrfToken = cookieCollection["BPMCSRF"].Value;
                request.CookieContainer = AuthCookie;
                request.Headers.Add("BPMCSRF", csrfToken);
                request.Headers.Set("ForceUseSession", "true");

                using (var writer = XmlWriter.Create(request.GetRequestStream()))
                {
                    entry.WriteTo(writer);
                }

                using (WebResponse responseCollection = request.GetResponse())
                {

                    responseCollection.Close();
                    return true;
                }
            }
            catch { }
            return false;

        }
        private bool UpdateByteEntityFieldById(string table, string field, string Id, byte[] file)
        {
            try
            {

                var request = (HttpWebRequest)HttpWebRequest.Create(serverUriUsr + table + "Collection(guid'" + Id.ToString() + "')/" + field);
                request.Method = "PUT";
                request.Accept = "application/octet-stream,application/json;odata=verbose";
                request.ContentType = "multipart/form-data;boundary=+++++";
                CookieCollection cookieCollection = AuthCookie.GetCookies(new Uri(authServiceUri));
                string csrfToken = cookieCollection["BPMCSRF"].Value;
                request.CookieContainer = AuthCookie;
                request.Headers.Add("BPMCSRF", csrfToken);
                request.Headers.Set("ForceUseSession", "true");

                using (Stream requestStream = request.GetRequestStream())
                {
                    requestStream.Write(file, 0, file.Length);
                }

                using (WebResponse responseCollection = request.GetResponse())
                {
                    responseCollection.Close();
                    return true;
                }

            }
            catch (Exception ex)
            {
                throw (ex);
            }

        }
        public string GetTImeZoneByContactId(string contactId)
        {
            try
            {

                string requestUri = serverUriUsr + "SysAdminUnitCollection?$filter=Contact/Id eq guid'" + contactId + "'&select=TimeZoneId";
                var request = HttpWebRequest.Create(requestUri) as HttpWebRequest;
                request.Method = "GET";
                CookieCollection cookieCollection = AuthCookie.GetCookies(new Uri(authServiceUri));
                string csrfToken = cookieCollection["BPMCSRF"].Value;
                request.CookieContainer = AuthCookie;
                request.Headers.Add("BPMCSRF", csrfToken);
                request.Headers.Set("ForceUseSession", "true");
                using (var response = request.GetResponse())
                {
                    if (((HttpWebResponse)response).StatusCode == HttpStatusCode.OK)
                    {
                        XDocument xmlDoc = XDocument.Load(response.GetResponseStream());
                        response.Close();

                        var items = from entry in xmlDoc.Descendants(ds + "TimeZoneId")
                                    select entry.Value;

                        return items.FirstOrDefault();
                    }
                }

            }
            catch
            {

            }
            return string.Empty;

        }
        #endregion

        #region Account
        public string CreateAccount(Account account)
        {
            try
            {
                Guid id = Guid.NewGuid();

                var content = new XElement(dsmd + "properties",
                            new XElement(ds + "Id", id.ToString()),
                            new XElement(ds + "Name", account.Name),
                            new XElement(ds + "Phone", account.PhoneNumber),
                            new XElement(ds + "AccountCategoryId", account.CategoryId),
                            new XElement(ds + "UsrCategoryOthers", account.CategoryOthers),
                            new XElement(ds + "UsrAboutTheAccount", account.AboutThe),
                            new XElement(ds + "Address", account.oAddress.Street),
                            new XElement(ds + "Zip", account.oAddress.Zip),
                            new XElement(ds + "AddressTypeId", ConfigurationManager.AppSettings["AddressTypeId"].ToString()),
                            new XElement(ds + "CityId", account.oAddress.CityId),
                            new XElement(ds + "CountryId", ConfigurationManager.AppSettings["CountryId"].ToString()),
                            new XElement(ds + "RegionId", account.oAddress.StateId),
                            new XElement(ds + "UsrImportSourceId", ConfigurationManager.AppSettings["ImportSourceID"].ToString())
                            );

                var entry = new XElement(atom + "entry",
                        new XElement(atom + "content",
                        new XAttribute("type", "application/xml"), content));

                var request = (HttpWebRequest)HttpWebRequest.Create(serverUriUsr + "AccountCollection/");
                CookieCollection cookieCollection = AuthCookie.GetCookies(new Uri(authServiceUri));
                string csrfToken = cookieCollection["BPMCSRF"].Value;
                request.CookieContainer = AuthCookie;
                request.Headers.Add("BPMCSRF", csrfToken);
                request.Headers.Set("ForceUseSession", "true");
                request.Method = "POST";
                request.Accept = "application/atom+xml";
                request.ContentType = "application/atom+xml;type=entry";
                using (var writer = XmlWriter.Create(request.GetRequestStream()))
                {
                    entry.WriteTo(writer);
                }

                using (WebResponse responseFileCollection = request.GetResponse())
                {
                    if (((HttpWebResponse)responseFileCollection).StatusCode == HttpStatusCode.Created)
                    {
                        responseFileCollection.Close();
                        return id.ToString();
                    }
                    else
                    { return string.Empty; }
                }
            }
            catch (Exception ex)
            {
                throw (ex);
            }

        }

        public bool UpdateAccount(Account account)
        {
            try
            {

                var content = new XElement(dsmd + "properties",
                              new XElement(ds + "Name", account.Name),
                              new XElement(ds + "Phone", account.PhoneNumber),
                              new XElement(ds + "AccountCategoryId", account.CategoryId),
                              new XElement(ds + "UsrCategoryOthers", account.CategoryOthers),
                              new XElement(ds + "UsrAboutTheAccount", account.AboutThe),
                              new XElement(ds + "Address", account.oAddress.Street),
                              new XElement(ds + "Zip", account.oAddress.Zip),
                              new XElement(ds + "AddressTypeId", ConfigurationManager.AppSettings["AddressTypeId"].ToString()),
                              new XElement(ds + "CityId", account.oAddress.CityId),
                              new XElement(ds + "CountryId", ConfigurationManager.AppSettings["CountryId"].ToString()),
                              new XElement(ds + "RegionId", account.oAddress.StateId),
                              new XElement(ds + "UsrImportSourceId", ConfigurationManager.AppSettings["ImportSourceID"].ToString())
                              );

                var entry = new XElement(atom + "entry",
                new XElement(atom + "content",
                    new XAttribute("type", "application/xml"),
                        content)
                );


                var request = (HttpWebRequest)HttpWebRequest.Create(serverUriUsr + "AccountCollection(guid'" + account.Id + "')");
                request.Method = "PUT";
                request.Accept = "application/atom+xml";
                request.ContentType = "application/atom+xml;type=entry";
                CookieCollection cookieCollection = AuthCookie.GetCookies(new Uri(authServiceUri));
                string csrfToken = cookieCollection["BPMCSRF"].Value;
                request.CookieContainer = AuthCookie;
                request.Headers.Add("BPMCSRF", csrfToken);
                request.Headers.Set("ForceUseSession", "true");

                using (var writer = XmlWriter.Create(request.GetRequestStream()))
                {
                    entry.WriteTo(writer);
                }

                using (WebResponse responseCollection = request.GetResponse())
                {
                    responseCollection.Close();
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }
        #endregion

        #region Contact

        public string CreateContact(Contact contact)
        {
            try
            {
                Guid id = Guid.NewGuid();

                var content = new XElement(dsmd + "properties",
                        new XElement(ds + "Id", id.ToString()),
                        new XElement(ds + "Name", contact.FullName),
                        new XElement(ds + "Email", contact.Email),
                        new XElement(ds + "Phone", contact.PhoneNumber),
                        new XElement(ds + "UsrBestWayToContactId", contact.BestWayToContactId),
                        new XElement(ds + "Address", contact.oAddress.Street),
                        new XElement(ds + "Zip", contact.oAddress.Zip),
                        new XElement(ds + "AddressTypeId", ConfigurationManager.AppSettings["AddressTypeId"].ToString()),
                        new XElement(ds + "CityId", contact.oAddress.CityId),
                        new XElement(ds + "CountryId", ConfigurationManager.AppSettings["CountryId"].ToString()),
                        new XElement(ds + "RegionId", contact.oAddress.StateId),
                        new XElement(ds + "AccountId", contact.AccountId),
                        new XElement(ds + "UsrImportSourceId", ConfigurationManager.AppSettings["ImportSourceID"].ToString())
                    );

                var entry = new XElement(atom + "entry",
                        new XElement(atom + "content",
                        new XAttribute("type", "application/xml"), content));

                var request = (HttpWebRequest)HttpWebRequest.Create(serverUriUsr + "ContactCollection/");
                CookieCollection cookieCollection = AuthCookie.GetCookies(new Uri(authServiceUri));
                string csrfToken = cookieCollection["BPMCSRF"].Value;
                request.CookieContainer = AuthCookie;
                request.Headers.Add("BPMCSRF", csrfToken);
                request.Headers.Set("ForceUseSession", "true");
                request.Method = "POST";
                request.Accept = "application/atom+xml";
                request.ContentType = "application/atom+xml;type=entry";
                using (var writer = XmlWriter.Create(request.GetRequestStream()))
                {
                    entry.WriteTo(writer);
                }

                using (WebResponse responseFileCollection = request.GetResponse())
                {
                    if (((HttpWebResponse)responseFileCollection).StatusCode == HttpStatusCode.Created)
                    {
                        responseFileCollection.Close();
                        return id.ToString();
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
            }
            catch (Exception ex)
            {
                throw (ex);
            }

        }

        public bool UpdateConctact(Contact contact)
        {
            try
            {
                var content = new XElement(dsmd + "properties",
                        new XElement(ds + "Name", contact.FullName),
                        new XElement(ds + "Email", contact.Email),
                        new XElement(ds + "Phone", contact.PhoneNumber),
                        new XElement(ds + "UsrBestWayToContactId", contact.BestWayToContactId),
                        new XElement(ds + "Address", contact.oAddress.Street),
                        new XElement(ds + "Zip", contact.oAddress.Zip),
                        new XElement(ds + "AddressTypeId", ConfigurationManager.AppSettings["AddressTypeId"].ToString()),
                        new XElement(ds + "CityId", contact.oAddress.CityId),
                        new XElement(ds + "CountryId", ConfigurationManager.AppSettings["CountryId"].ToString()),
                        new XElement(ds + "RegionId", contact.oAddress.StateId),
                        new XElement(ds + "UsrImportSourceId", ConfigurationManager.AppSettings["ImportSourceID"].ToString())
                 );


                if (!string.IsNullOrEmpty(contact.AccountId))
                {
                    content.Add(new XElement(ds + "AccountId", contact.AccountId));
                }

                var entry = new XElement(atom + "entry",
                        new XElement(atom + "content",
                            new XAttribute("type", "application/xml"),
                                content)
                        );

                var request = (HttpWebRequest)HttpWebRequest.Create(serverUriUsr + "ContactCollection(guid'" + contact.Id + "')");
                request.Method = "PUT";
                request.Accept = "application/atom+xml";
                request.ContentType = "application/atom+xml;type=entry";
                CookieCollection cookieCollection = AuthCookie.GetCookies(new Uri(authServiceUri));
                string csrfToken = cookieCollection["BPMCSRF"].Value;
                request.Headers.Set("ForceUseSession", "true");
                request.CookieContainer = AuthCookie;
                request.Headers.Add("BPMCSRF", csrfToken);

                using (var writer = XmlWriter.Create(request.GetRequestStream()))
                {
                    entry.WriteTo(writer);
                }

                using (WebResponse responseCollection = request.GetResponse())
                {

                    responseCollection.Close();
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }


        #endregion

        #region Organization - Contact
        private bool ExistsOrganizationContact(string accountId, string contactId)
        {
            try
            {

                string requestUri = serverUriUsr + "UsrOrganizationContactCollection?$filter=UsrOrganization/Id eq guid'" + accountId + "' and UsrContact/Id eq guid'" + contactId + "'";
                var request = HttpWebRequest.Create(requestUri) as HttpWebRequest;
                request.Method = "GET";
                CookieCollection cookieCollection = AuthCookie.GetCookies(new Uri(authServiceUri));
                string csrfToken = cookieCollection["BPMCSRF"].Value;
                request.CookieContainer = AuthCookie;
                request.Headers.Add("BPMCSRF", csrfToken);
                request.Headers.Set("ForceUseSession", "true");
                using (var response = request.GetResponse())
                {
                    if (((HttpWebResponse)response).StatusCode == HttpStatusCode.OK)
                    {
                        XDocument xmlDoc = XDocument.Load(response.GetResponseStream());
                        response.Close();

                        var items = from entry in xmlDoc.Descendants(ds + "Id")
                                    select entry.Value;

                        return (items.FirstOrDefault() == null ? false : true);
                    }
                }

            }
            catch
            {

            }
            return false;
        }
        private bool CreateOrganizationContact(string accountId, string contactId)
        {
            try
            {

                Guid id = Guid.NewGuid();

                var content = new XElement(dsmd + "properties",
                        new XElement(ds + "Id", id.ToString()),
                        new XElement(ds + "UsrOrganizationId", accountId),
                        new XElement(ds + "UsrContactId", contactId),
                        new XElement(ds + "UsrImportSourceId", ConfigurationManager.AppSettings["ImportSourceID"].ToString())
                    );

                var entry = new XElement(atom + "entry",
                        new XElement(atom + "content",
                        new XAttribute("type", "application/xml"), content));

                var request = (HttpWebRequest)HttpWebRequest.Create(serverUriUsr + "UsrOrganizationContactCollection/");
                CookieCollection cookieCollection = AuthCookie.GetCookies(new Uri(authServiceUri));
                string csrfToken = cookieCollection["BPMCSRF"].Value;
                request.CookieContainer = AuthCookie;
                request.Headers.Add("BPMCSRF", csrfToken);
                request.Headers.Set("ForceUseSession", "true");
                request.Method = "POST";
                request.Accept = "application/atom+xml";
                request.ContentType = "application/atom+xml;type=entry";
                using (var writer = XmlWriter.Create(request.GetRequestStream()))
                {
                    entry.WriteTo(writer);
                }

                using (WebResponse responseFileCollection = request.GetResponse())
                {
                    if (((HttpWebResponse)responseFileCollection).StatusCode == HttpStatusCode.Created)
                    {
                        responseFileCollection.Close();
                        return true;
                    }
                }
            }
            catch { }
            return false;

        }
        #endregion

        #region Preferred Restaurant - Contact

        private bool ExistsPreferredRestaurantContact(string contactId, string preferredRestaurantId)
        {
            try
            {

                string requestUri = serverUriUsr + "UsrPreferredRestaurantsCollection?$filter=UsrContact/Id eq guid'" + contactId + "'and UsrRestaurant/Id eq guid'" + preferredRestaurantId + "'";
                var request = HttpWebRequest.Create(requestUri) as HttpWebRequest;
                request.Method = "GET";
                CookieCollection cookieCollection = AuthCookie.GetCookies(new Uri(authServiceUri));
                string csrfToken = cookieCollection["BPMCSRF"].Value;
                request.CookieContainer = AuthCookie;
                request.Headers.Add("BPMCSRF", csrfToken);
                request.Headers.Set("ForceUseSession", "true");
                using (var response = request.GetResponse())
                {
                    if (((HttpWebResponse)response).StatusCode == HttpStatusCode.OK)
                    {
                        XDocument xmlDoc = XDocument.Load(response.GetResponseStream());
                        response.Close();

                        var items = from entry in xmlDoc.Descendants(ds + "Id")
                                    select entry.Value;

                        return (items.FirstOrDefault() == null ? false : true);
                    }
                }

            }
            catch
            {

            }

            return false;
        }

        private bool CreatePreferredRestaurantContact(string contactId, string preferredRestaurantId)
        {
            try
            {

                Guid id = Guid.NewGuid();

                var content = new XElement(dsmd + "properties",
                        new XElement(ds + "Id", id.ToString()),
                        new XElement(ds + "UsrRestaurantId", preferredRestaurantId),
                        new XElement(ds + "UsrContactId", contactId)
                    );

                var entry = new XElement(atom + "entry",
                        new XElement(atom + "content",
                        new XAttribute("type", "application/xml"), content));

                var request = (HttpWebRequest)HttpWebRequest.Create(serverUriUsr + "UsrPreferredRestaurantsCollection/");
                CookieCollection cookieCollection = AuthCookie.GetCookies(new Uri(authServiceUri));
                string csrfToken = cookieCollection["BPMCSRF"].Value;
                request.CookieContainer = AuthCookie;
                request.Headers.Add("BPMCSRF", csrfToken);
                request.Headers.Set("ForceUseSession", "true");
                request.Method = "POST";
                request.Accept = "application/atom+xml";
                request.ContentType = "application/atom+xml;type=entry";
                using (var writer = XmlWriter.Create(request.GetRequestStream()))
                {
                    entry.WriteTo(writer);
                }

                using (WebResponse responseCollection = request.GetResponse())
                {
                    if (((HttpWebResponse)responseCollection).StatusCode == HttpStatusCode.Created)
                    {
                        responseCollection.Close();
                        return true;
                    }
                }
            }
            catch { }

            return false;

        }

        #endregion

        #region Case

        public string CreateCase(Case _Case, Account _Account)
        {

            try
            {
                Guid id = Guid.NewGuid();
                XElement content;
                if (_Case.PreferredRestaurant != "" && _Case.PreferredRestaurant != null)
                {
                    content = new XElement(dsmd + "properties",
                    new XElement(ds + "Id", id.ToString()),
                    new XElement(ds + "Subject", _Case.Subject),
                    new XElement(ds + "UsrAnotherAddress", _Case.AnotherAddress),
                    new XElement(ds + "UsrSupportedThisEventInThePast", _Case.SupportedThisEventInThePast),
                    new XElement(ds + "UsrSupportedThisEventInThePastHow", _Case.SupportedThisEventInThePastHow),
                    new XElement(ds + "UsrHowManyPeopletoAttendSupport", _Case.HowManyPeopletoAttendSupport),
                    new XElement(ds + "UsrIdeaRCToHelp", _Case.IdeaRCToHelp),
                    new XElement(ds + "UsrAdditionalInfoRCShoudKnow", _Case.AdditionalInfoRCShoudKnow),
                    new XElement(ds + "UsrPurposeOfTheEvent", _Case.PurposeOfTheEvent),
                    new XElement(ds + "AccountId", _Case.AccountId),
                    new XElement(ds + "ContactId", _Case.ContactId),
                    new XElement(ds + "UsrEventAddressId", _Case.EventAddressId),
                    new XElement(ds + "UsrRestaurantId", _Case.PreferredRestaurant),
                    new XElement(ds + "UsrImportSourceId", ConfigurationManager.AppSettings["ImportSourceID"].ToString())
                    );
                }
                else
                {
                    content = new XElement(dsmd + "properties",
                    new XElement(ds + "Id", id.ToString()),
                    new XElement(ds + "Subject", _Case.Subject),
                    new XElement(ds + "UsrAnotherAddress", _Case.AnotherAddress),
                    new XElement(ds + "UsrSupportedThisEventInThePast", _Case.SupportedThisEventInThePast),
                    new XElement(ds + "UsrSupportedThisEventInThePastHow", _Case.SupportedThisEventInThePastHow),
                    new XElement(ds + "UsrHowManyPeopletoAttendSupport", _Case.HowManyPeopletoAttendSupport),
                    new XElement(ds + "UsrIdeaRCToHelp", _Case.IdeaRCToHelp),
                    new XElement(ds + "UsrAdditionalInfoRCShoudKnow", _Case.AdditionalInfoRCShoudKnow),
                    new XElement(ds + "UsrPurposeOfTheEvent", _Case.PurposeOfTheEvent),
                    new XElement(ds + "AccountId", _Case.AccountId),
                    new XElement(ds + "ContactId", _Case.ContactId),
                    new XElement(ds + "UsrEventAddressId", _Case.EventAddressId),
                    new XElement(ds + "UsrNewOrgAddress", _Account.oAddress.Street),
                    new XElement(ds + "UsrNewOrgCityId", _Account.oAddress.CityId),
                    new XElement(ds + "UsrNewOrgStateId", _Account.oAddress.StateId),
                    new XElement(ds + "UsrNewOrgZipCode", _Account.oAddress.Zip),
            new XElement(ds + "UsrImportSourceId", ConfigurationManager.AppSettings["ImportSourceID"].ToString())
                    );
                }




                string timeZone = GetTImeZoneByContactId(ConfigurationManager.AppSettings["ContactId"].ToString());
                DateTime dtTime = new DateTime(_Case.Date.Year, _Case.Date.Month, _Case.Date.Day, _Case.TimeEvent.Hours, _Case.TimeEvent.Minutes, 00);
                TimeZoneInfo userZone = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
                var dt = TimeZoneInfo.ConvertTimeToUtc(dtTime, userZone);

                content.Add(new XElement(ds + "UsrEventTime", dt));
                content.Add(new XElement(ds + "UsrEventDate", dt));



                if (_Case.AnotherAddress)
                {
                    content.Add(new XElement(ds + "UsrAddress", _Case.oAddress.Street));
                    content.Add(new XElement(ds + "UsrCityId", _Case.oAddress.CityId));
                    content.Add(new XElement(ds + "UsrStateId", _Case.oAddress.StateId));
                    content.Add(new XElement(ds + "UsrPostalCode", _Case.oAddress.Zip));
                }

                //Organization Address
                //content.Add(new XElement(ds + "UsrNewOrgAddress", _Account.oAddress.Street));
                //content.Add(new XElement(ds + "UsrNewOrgCityId", _Account.oAddress.CityId));
                //content.Add(new XElement(ds + "UsrNewOrgStateId", _Account.oAddress.StateId));
                //content.Add(new XElement(ds + "UsrNewOrgZipCode", _Account.oAddress.Zip));


                var entry = new XElement(atom + "entry",
                        new XElement(atom + "content",
                        new XAttribute("type", "application/xml"), content));

                var request = (HttpWebRequest)HttpWebRequest.Create(serverUriUsr + "CaseCollection/");
                CookieCollection cookieCollection = AuthCookie.GetCookies(new Uri(authServiceUri));
                string csrfToken = cookieCollection["BPMCSRF"].Value;
                request.CookieContainer = AuthCookie;
                request.Headers.Add("BPMCSRF", csrfToken);
                request.Headers.Set("ForceUseSession", "true");
                request.Method = "POST";
                request.Accept = "application/atom+xml";
                request.ContentType = "application/atom+xml;type=entry";
                using (var writer = XmlWriter.Create(request.GetRequestStream()))
                {
                    entry.WriteTo(writer);
                }

                using (WebResponse responseFileCollection = request.GetResponse())
                {
                    if (((HttpWebResponse)responseFileCollection).StatusCode == HttpStatusCode.Created)
                    {
                        responseFileCollection.Close();
                        return id.ToString();
                    }
                }

                return "";
            }
            catch (Exception ex)
            {
                throw (ex);
            }

        }

        #endregion

        #region Case Files

        public bool CreateCaseFile(Archive file)
        {
            try
            {
                Guid id = Guid.NewGuid();

                var content = new XElement(dsmd + "properties",
                            new XElement(ds + "Id", id.ToString()),
                            new XElement(ds + "Name", file.Name),
                            new XElement(ds + "CaseId", file.EntityId),
                            new XElement(ds + "TypeId", ConfigurationManager.AppSettings["FileTypeId"].ToString()),
                            new XElement(ds + "Size", file.Size)
                        );


                var entry = new XElement(atom + "entry",
                        new XElement(atom + "content",
                        new XAttribute("type", "application/xml"), content));

                var request = (HttpWebRequest)HttpWebRequest.Create(serverUriUsr + "CaseFileCollection/");
                CookieCollection cookieCollection = AuthCookie.GetCookies(new Uri(authServiceUri));
                string csrfToken = cookieCollection["BPMCSRF"].Value;
                request.CookieContainer = AuthCookie;
                request.Headers.Add("BPMCSRF", csrfToken);
                request.Headers.Set("ForceUseSession", "true");
                request.Method = "POST";
                request.Accept = "application/atom+xml";
                request.ContentType = "application/atom+xml;type=entry";
                using (var writer = XmlWriter.Create(request.GetRequestStream()))
                {
                    entry.WriteTo(writer);
                }

                using (WebResponse responseFileCollection = request.GetResponse())
                {
                    if (((HttpWebResponse)responseFileCollection).StatusCode == HttpStatusCode.Created)
                    {
                        responseFileCollection.Close();
                        UpdateByteEntityFieldById("CaseFile", "Data", id.ToString(), file.Content);
                        return true;
                    }
                }


            }
            catch
            {

            }

            return false;
        }

        #endregion

        #region City

        public string CreateCity(string Name, string RegionId)
        {
            try
            {
                Guid id = Guid.NewGuid();

                var content = new XElement(dsmd + "properties",
                        new XElement(ds + "Id", id.ToString()),
                        new XElement(ds + "Name", Name),
                        new XElement(ds + "CountryId", ConfigurationManager.AppSettings["CountryId"].ToString()),
                        new XElement(ds + "RegionId", RegionId));

                var entry = new XElement(atom + "entry",
                        new XElement(atom + "content",
                        new XAttribute("type", "application/xml"), content));

                var request = (HttpWebRequest)HttpWebRequest.Create(serverUriUsr + "CityCollection/");
                CookieCollection cookieCollection = AuthCookie.GetCookies(new Uri(authServiceUri));
                string csrfToken = cookieCollection["BPMCSRF"].Value;
                request.CookieContainer = AuthCookie;
                request.Headers.Add("BPMCSRF", csrfToken);
                request.Headers.Set("ForceUseSession", "true");
                request.Method = "POST";
                request.Accept = "application/atom+xml";
                request.ContentType = "application/atom+xml;type=entry";
                using (var writer = XmlWriter.Create(request.GetRequestStream()))
                {
                    entry.WriteTo(writer);
                }

                using (WebResponse responseFileCollection = request.GetResponse())
                {
                    if (((HttpWebResponse)responseFileCollection).StatusCode == HttpStatusCode.Created)
                    {
                        responseFileCollection.Close();
                        return id.ToString();
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
            }
            catch (Exception ex)
            {
                throw (ex);
            }

        }

        public string FindCityByNameAndRegionId(string name, string regionId)
        {
            try
            {
                string requestUri = serverUriUsr + "CityCollection?$filter=Name eq '" + name + "' and Region/Id eq guid'" + regionId + "'";
                var request = HttpWebRequest.Create(requestUri) as HttpWebRequest;
                request.Method = "GET";
                CookieCollection cookieCollection = AuthCookie.GetCookies(new Uri(authServiceUri));
                string csrfToken = cookieCollection["BPMCSRF"].Value;
                request.CookieContainer = AuthCookie;
                request.Headers.Add("BPMCSRF", csrfToken);
                request.Headers.Set("ForceUseSession", "true");
                using (var response = request.GetResponse())
                {
                    if (((HttpWebResponse)response).StatusCode == HttpStatusCode.OK)
                    {
                        XDocument xmlDoc = XDocument.Load(response.GetResponseStream());
                        response.Close();

                        var items = from entry in xmlDoc.Descendants(ds + "Id")
                                    select entry.Value;

                        return items.FirstOrDefault();
                    }
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                throw (ex);
            }



        }

        #endregion

    }
}





