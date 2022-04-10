using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Raisingcanes.Helper;
using Raisingcanes.Model;
using System.Configuration;
using System.Web.Services;
using System.Globalization;
using System.IO;

namespace Raisingcanes.Site
{
    public partial class Register : System.Web.UI.Page
    {
        protected void btnPageLoad_Click(object sender, EventArgs e)
        {

            this.divLoaderPage.Visible = false;
            this.hdnLoaded.Value = "true";
            this.divformDetail.Visible = true;

            try
            {
                using (OData helper = new OData())
                {
                    #region CBO Contacts - Best Way To Contact You

                    var ContactForms = helper.GetLookup("UsrContactsBestWayToContactYou", "Name");
                    this.cbContactForm.Items.Clear();
                    this.cbContactForm.Items.Add(new ListItem() { Value = "", Text = "", Selected = true });
                    foreach (var item in ContactForms)
                    {
                        this.cbContactForm.Items.Add(new ListItem() { Value = item.Id, Text = item.Text });
                    }

                    #endregion

                    #region CBO State

                    var States = helper.GetLookup("Region", "Name");
                    this.cbState.Items.Clear();
                    this.cbState.Items.Add(new ListItem() { Value = "", Text = "", Selected = true });
                    foreach (var item in States)
                    {
                        this.cbState.Items.Add(new ListItem() { Value = item.Id, Text = item.Text });
                    }

                    //HostEvent #57
                    var HostEvent = helper.GetLookup("UsrEventAddressType", "Name");
                    this.cbHostEvent.Items.Clear();
                    foreach (var item in HostEvent)
                    {
                        if (item.Id == "edbfdd08-96f8-4a9a-b636-808ae93677d0")
                        {
                            this.cbHostEvent.Items.Add(new ListItem() { Value = item.Id, Text = item.Text, Selected = true });
                        }
                        else if (item.Id == "91bf9e7a-9ffd-45df-b972-22c08e02497e")
                        {

                        }
                        else
                        {
                            this.cbHostEvent.Items.Add(new ListItem() { Value = item.Id, Text = item.Text });
                        }
                    }

                    //this.cbEventState.Items.Clear();
                    //this.cbEventState.Items.Add(new ListItem() { Value = "", Text = "", Selected = true });
                    //foreach (var item in States)
                    //{
                    //    this.cbEventState.Items.Add(new ListItem() { Value = item.Id, Text = item.Text });
                    //}


                    //this.cbOtherRestaurant.Items.Clear();
                    //this.cbOtherRestaurant.Items.Add(new ListItem() { Value = "", Text = "", Selected = true });
                    //foreach (var item in States)
                    //{
                    //    this.cbOtherRestaurant.Items.Add(new ListItem() { Value = item.Id, Text = item.Text });
                    //}


                    #endregion

                    #region CBO Account Category

                    var AccCategories = helper.GetLookup("AccountCategory", "Name");
                    this.cbAccountCategory.Items.Clear();
                    this.cbAccountCategory.Items.Add(new ListItem() { Value = "", Text = "", Selected = true });
                    foreach (var item in AccCategories)
                    {
                        this.cbAccountCategory.Items.Add(new ListItem() { Value = item.Id, Text = item.Text });
                    }

                    #endregion

                    #region Restaurant

                    var Restaurants = helper.GetLookup("UsrRestaurants", "UsrRestaurantName");
                    this.cbPreferredRestaurant.Items.Clear();
                    this.cbPreferredRestaurant.Items.Add(new ListItem() { Value = "", Text = "", Selected = true });
                    foreach (var item in Restaurants)
                    {
                        this.cbPreferredRestaurant.Items.Add(new ListItem() { Value = item.Id, Text = item.Text });
                    }

                    var availableRestaurants = GetAvailableRestaurants();
                    if (!string.IsNullOrEmpty(availableRestaurants)) {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "AvailableRestaurants", $"<script>const raisingRestaurants = {availableRestaurants};</script>", false);
                    }



                    #endregion




                }
                this.wrapped.Visible = true;
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ModalView", "<script>$(function() { $('#ModalError').modal('open'); alert('" + ex.Message + "'); });</script>", false);
            }


        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnRegister_Click(object sender, EventArgs e)
        {

            try
            {

                Data _Data = new Data();
                OData _OData = new OData();
                _Data.oAccount = new Account();
                _Data.oContact = new Contact();
                _Data.oCase = new Case();


                _Data.oContact.FullName = this.txtFirstName.Text.Trim() + " " + this.txtLastName.Text.Trim();
                _Data.oContact.PhoneNumber = this.txtContactPhone.Text.Trim();
                _Data.oContact.Email = this.txtEmail.Text.Trim();
                _Data.oContact.BestWayToContactId = this.cbContactForm.SelectedValue;

                _Data.oContact.oAddress = new Address();
                _Data.oContact.oAddress.City = this.txtCity.Text.Trim();
                _Data.oContact.oAddress.Street = this.txtStreet.Text.Trim();
                _Data.oContact.oAddress.Zip = this.txtZip.Text.Trim();
                _Data.oContact.oAddress.StateId = this.cbState.SelectedValue;
                //_Data.oContact.PreferredRestaurant = this.InpPreferredRestaurant.Value;
                //_Data.oCase.PreferredRestaurant = this.InpPreferredRestaurant.Value;

                _Data.oAccount.Name = this.txtAccount.Text.Trim();
                _Data.oAccount.PhoneNumber = this.txtAccountPhone.Text.Trim();
                _Data.oAccount.CategoryId = this.cbAccountCategory.SelectedValue;
                _Data.oAccount.CategoryOthers = this.txtAccountCategoryOthers.Text.Trim();
                _Data.oAccount.AboutThe = this.txtAboutTheAccount.Text.Trim();
                _Data.oAccount.oAddress = _Data.oContact.oAddress;

                _Data.oCase.Subject = this.txtEventDescription.Text.Trim();

                DateTimeFormatInfo usDtfi = new CultureInfo("en-US", false).DateTimeFormat;
                _Data.oCase.Date = Convert.ToDateTime(this.txtEventDate.Value, usDtfi);
                //if (DateTime.TryParse(this.txtEventDate.Value, out _valueDate))
                //    _Data.oCase.Date = _valueDate;

                if (this.EventTime1.Value != "" && this.EventTime1.Value != null)
                {
                    string[] strValue = this.EventTime1.Value.Split(':');
                    int hour = Convert.ToInt32(strValue[0].Trim());
                    int min = Convert.ToInt32(strValue[1].Trim().Substring(0, 2));
                    if (strValue[1].Contains("PM") && hour < 12)
                        hour += 12;
                    _Data.oCase.TimeEvent = new TimeSpan(hour, min, 0);
                }

                if (!string.IsNullOrEmpty(this.txtEventCity.Text))
                {
                    _Data.oCase.oAddress = new Address();
                    _Data.oCase.oAddress.City = this.txtEventCity.Text.Trim();
                    _Data.oCase.oAddress.Street = this.txtEventStreet.Text.Trim();
                    _Data.oCase.oAddress.Zip = this.txtEventZip.Text.Trim();
                    _Data.oCase.oAddress.StateId = _OData.GetEntityIdByField("Region", "Name", this.txtEventState.Text.Trim());
                    _Data.oCase.AnotherAddress = true;
                }
                else
                {
                    _Data.oCase.AnotherAddress = false;
                }


                if (!string.IsNullOrEmpty(this.txtHowSupportedEvent.Text))
                {

                    _Data.oCase.SupportedThisEventInThePast = true;
                    _Data.oCase.SupportedThisEventInThePastHow = this.txtHowSupportedEvent.Text.Trim();
                }
                else
                {
                    _Data.oCase.SupportedThisEventInThePast = false;
                }
                _Data.oCase.PurposeOfTheEvent = this.txtPurposeEvent.Text.Trim();

                /*Fix 02/20/2020 - Eric Garcia de Campos
                 * The page was not accepting special characters.                
                 * int number = 0;
                 * if (Int32.TryParse(this.txtPeopleEvent.Text, out number))
                 * _Data.oCase.HowManyPeopletoAttendSupport = number;*/
                _Data.oCase.HowManyPeopletoAttendSupport = this.txtPeopleEvent.Text.Trim();

                _Data.oCase.IdeaRCToHelp = this.txtIdeasRCEvent.Text.Trim();
                _Data.oCase.AdditionalInfoRCShoudKnow = this.txtAdditionalInformationEvent.Text.Trim();
                _Data.oCase.EventAddressId = this.cbHostEvent.SelectedValue;

                //if (this.RdOtherAddressAtRestaurant.Checked == true)
                //{
                //    _Data.oCase.EventAddressId = ConfigurationManager.AppSettings["AddressAtRestaurantId"].ToString();
                //}
                //else if (this.RdOtherAddress.Checked == true)
                //{
                //    _Data.oCase.EventAddressId = ConfigurationManager.AppSettings["AddressOtherID"].ToString();
                //}
                //else
                //{
                //    _Data.oCase.EventAddressId = ConfigurationManager.AppSettings["AddressOrgId"].ToString();
                //}


                for (int i = 0; i < Request.Files.Count; i++)
                {
                    System.Web.HttpPostedFile postedFile = Request.Files[i];

                    if (!string.IsNullOrEmpty(postedFile.FileName))
                    {
                        Archive _file = new Archive();
                        _file.Name = postedFile.FileName;
                        System.IO.Stream inStream = postedFile.InputStream;
                        byte[] fileData = new byte[postedFile.ContentLength];
                        inStream.Read(fileData, 0, postedFile.ContentLength);
                        _file.Name = Path.GetFileName(postedFile.FileName);
                        _file.Content = fileData;
                        _file.Size = postedFile.ContentLength;

                        if (Request.Files.AllKeys[i] == "chooseFileAdditionalInf")
                            _Data.oCase.FileAdditional = _file;
                        else
                            _Data.oCase.FileW9 = _file;
                    }
                }


                using (var ohelper = new OData())
                {
                    if (ohelper.SaveData(_Data))
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ModalView", "<script>$(function() { $('#ModalOk').modal('open'); $('#cbContactForm').css({ 'z-index': '0', 'display': 'block'}); });</script>", false);
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ModalView", "<script>$(function() { $('#ModalError').modal('open'); });</script>", false);
                    }

                }

            }
            catch (Exception)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ModalView", "<script>$(function() { $('#ModalError').modal('open'); });</script>", false);
            }
            finally
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ModalView", "<script>$(function() { $('#ModalOk').modal('open'); });</script>", false);
            }
        }

        [WebMethod]
        public static string GetRestaurants(string Id)
        {
            using (OData helper = new OData())
            {
                var RestaurantsList = helper.GetRestaurants("UsrRestaurants", "Id", Id);
                return RestaurantsList;
            }
        }
        protected override void InitializeCulture()
        {
            UICulture = "en-us";
            base.InitializeCulture();
        }




        [WebMethod]
        public static string GetPositionByCustomerAddress(string state, string city, string address)
        {
            using (OData helper = new OData())
            {
                var position = helper.GetPositionByAddress("United States", state, city, address);
                return position;
            }
        }


        public string GetAvailableRestaurants()
        {
            using (OData helper = new OData())
            {
                var RestaurantsList = helper.GetRestaurantsAvailables();
                return RestaurantsList;
            }
        }








    }
}