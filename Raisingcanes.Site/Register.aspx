<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Register.aspx.cs" EnableEventValidation="false" Inherits="Raisingcanes.Site.Register" %>

<!DOCTYPE html>
<html lang="en">

<head>

    <meta charset="utf-8">
    <title>Community Request System | Raising Cane's</title>
    <meta name="description" content="Community Request System">
    <meta name="author" content="Raising Cane's">

    <!-- Mobile Specific Metas -->
    <meta name="viewport" content="width=device-width, initial-scale=1.0">

    <!-- CSS -->
    <link href="theme/css/style.css" rel="stylesheet">
    <link href="theme/css/materialize.css" rel="stylesheet">
    <link href="theme/ericjgagnon-wickedpicker-2a8950a/stylesheets/wickedpicker.css" rel="stylesheet">

    <!-- New CSS -->

    <!-- Jquery -->
    <script src="theme/js/jquery-1.10.2.min.js"></script>
    <script src="theme/js/jquery-ui-1.8.22.min.js"></script>
    <script src="theme/js/jquery.mask.min.js"></script>
    <script src="theme/ericjgagnon-wickedpicker-2a8950a/src/wickedpicker.js"></script>
    <!-- New Jquery -->
    <script src="https://cdnjs.cloudflare.com/ajax/libs/materialize/1.0.0/js/materialize.min.js"></script>
    <script src="theme/js/functions.js"></script>
    <script type="text/javascript">
        var _isInitialLoad = true;
        function PageLoad() {
            if (_isInitialLoad) {
                if ($('#hdnLoaded').val())
                    return false;
                _isInitialLoad = false;
                setTimeout('__doPostBack(\'<%= this.btnPageLoad.ClientID %>\',\'\');', 100);
            }
        }
    </script>
    <script>
        $(function () {
            
            $('#EventTime1').wickedpicker();

            $("#txtEventDate").datepicker({
                format: 'mm/dd/yyyy',
                selectMonths: true,
                yearRange: 80,
                dismissible: false,
                minDate: new Date(),
                closeOnSelect: true                                             
            });
            //$("#EventTime").timepicker();
            $("#chooseFileW9").on("change", function (e) {
                debugger;
                var fileOk = true;
                var files = e.currentTarget.files;
                // call them as such; files[0].size will get you the file size of the 0th file
                for (var x in files) {
                    var filesize = ((files[x].size/1024)/1024).toFixed(4); // MB
                    if (filesize > <%=ConfigurationManager.AppSettings["SizeLimitMB"] %>)
                        fileOk = false;
                }
            
                if (!fileOk) {
                    $("#chooseFileW9").val('');
                    $("#noFileW9").text("Files must not exceed the limit size of " +<%=ConfigurationManager.AppSettings["SizeLimitMB"] %>+"Mb");
                }
            });
            $("#chooseFileAdditionalInf").on("change", function (e) {
                debugger;
                var files = e.currentTarget.files;
                var fileOk = true;
                // call them as such; files[0].size will get you the file size of the 0th file
                for (var x in files) {

                    var filesize = ((files[x].size/1024)/1024).toFixed(4); // MB
                    console.log(filesize);
                    if (filesize > <%=ConfigurationManager.AppSettings["SizeLimitMB"] %>)
                        fileOk = false;
                }
            
                if (!fileOk) {
                    $("#chooseFileAdditionalInf").val('');
                    $("#noFileAdditionalInf").text("Files must not exceed the limit size of " +<%=ConfigurationManager.AppSettings["SizeLimitMB"] %>+"Mb");
                }
            });
            $("#cbHostEvent").on("change", (e) => {
                let target = $(e.currentTarget);
                console.log(target.val());
            });
        });

        function showMap() {

            alert("You can move search center, change radius and click on the restaurant on the map to select");
        }

        
    </script>


</head>
<body onload="PageLoad()">
    <div id="leftHalf"></div>
    <div id="rightHalf"></div>

    <%--<div class="container" id="menucontainer">
        <h1>Menu Area</h1>
    </div>--%>

    <div class="container" id="headercontainer">
        <div class="logo-img">
            <a href="http://www.raisingcanes.com" rel="home">
                <img src="https://raisingcanes.com/sites/default/files/logo_raising_cane.png" alt="Raising Cane's Chicken Fingers">
            </a>
        </div>
        <h1 class="title-header">COMMUNITY REQUEST SYSTEM</h1>
        <br>
        <br>
    </div>

    <div class="container" id="maincontainer">

        <div class="column" id="secondarycontainer">

            <form name="frmcontacto" id="wrapped" runat="server" enctype="multipart/form-data">
                <asp:ScriptManager ID="sm" EnablePageMethods="true" runat="server" />
                <asp:Button ID="btnPageLoad" runat="server" Text="Page Load" OnClick="btnPageLoad_Click" Style="display: none" UseSubmitBehavior="false" />
                <input type="hidden" id="hdnLoaded" runat="server" />
                <div id="divformDetail" runat="server" visible="false">
                    <div class="rowData">
                        <table>
                            <tr>
                                <td>
                                    <h2 class="title-header">Thank you for considering Raising Cane’s Chicken Fingers® for your request. </h2>
                                    <br />
                                    <p>At Raising Cane’s we pride ourselves in supporting our community. We work hard to be involved in as many philanthropic and community groups, programs, events and partnerships as we can. This ranges from providing a gift card for a door prize at a philanthropic event all the way to building an entire dog park!</p>
                                    <br />
                                    <p>Because of the great amount of requests we receive, please submit your request at least 3-5 weeks prior to the date of your event and complete all fields so we can respond in a timely manner.</p>
                                    <br />
                                    <p>To get started with your request fill out the form below and a Raising Cane’s Crew Member will get in touch with you soon to discuss how we can provide support.</p>
                                </td>
                            </tr>
                        </table>

                        <br />
                        <p><span style="color: #D51A2C; font-style: italic"><em>Fields marked with * are required</em></span></p>

                        <h3>Contact Information:</h3>

                        <div>
                            <div class="contact-container">
                                <label class="t-label-contact t-label-is-required"><%=Resources.Messages.ContactFirstName%></label>                            
                                <br class="linebreak1"/>
                                <div class="base-edit-contact ts-box-sizing edit base-edit-error">
                                    <asp:TextBox ID="txtFirstName" CssClass="base-edit-input ts-box-sizing required" MaxLength="125" runat="server" />
                                </div>
                            </div>
                            <br class="mobile" />
                            <br class="mobile" />
                            <div class="contact-container">
                                <label class="t-label-contact t-label-is-required"><%=Resources.Messages.ContactEmail%></label>
                                <br class="linebreak1"/>
                                <div class="base-edit-contact ts-box-sizing edit base-edit-error">
                                    <asp:TextBox ID="txtEmail" type="email" CssClass="base-edit-input ts-box-sizing required" MaxLength="250" runat="server" />
                                </div>                                
                            </div>
                            

                            <br />
                            <br />
                        </div>


                        <div>
                            <div class="contact-container">
                                <label class="t-label-contact t-label-is-required"><%=Resources.Messages.ContactLastName%></label>
                                <br class="linebreak1"/>
                                <div class="base-edit-contact ts-box-sizing edit base-edit-error">
                                    <asp:TextBox ID="txtLastName" CssClass="base-edit-input ts-box-sizing required" MaxLength="125" runat="server" />
                                </div>
                            </div>                            
                            <br class="mobile" />
                            <br class="mobile" />
                            <div class="contact-container">
                                <label class="t-label-contact t-label-is-required"><%=Resources.Messages.ContactEmailConfirm%></label>
                                <br class="linebreak1"/>
                                <div class="base-edit-contact ts-box-sizing edit base-edit-error">
                                    <asp:TextBox ID="txtEmail2" type="email" CssClass="base-edit-input ts-box-sizing" MaxLength="250" runat="server" />
                                </div>
                            </div>

                            <br />
                            <br />
                        </div>

                        <div>
                            <div class="contact-container">
                                <label class="t-label-contact t-label-is-required"><%=Resources.Messages.ContactPhone%></label>
                                <br />
                                <div class="base-edit-contact ts-box-sizing edit base-edit-error">
                                    <asp:TextBox ID="txtContactPhone" CssClass="base-edit-input ts-box-sizing required" MaxLength="12" runat="server" />
                                </div>
                            </div>
                            <br class="mobile" />
                            <br class="mobile" />
                            <div class="contact-container">
                                <label class="t-label-contact t-label-is-required" style="padding-bottom:6px"><%=Resources.Messages.ContactForm%></label>
                                <br class="linebreak1"/>
                                <div class="select-option select-option-error">
                                    <asp:DropDownList ID="cbContactForm" CssClass="select-option-detail required" runat="server"></asp:DropDownList>
                                </div>
                            </div>

                            <br />
                            <% if (!HttpContext.Current.Request.UserAgent.Contains("Edge"))
                                { %>
                            <br />
                            <% } %>                            
                            
                        </div>                        

                        <h3>Organization & Event Information</h3>
                        <div>
                            <label class="t-label t-label-is-required"><%=Resources.Messages.AccountName%></label>
                            <br />
                            <div class="base-edit ts-box-sizing edit base-edit-error">
                                <asp:TextBox ID="txtAccount" CssClass="base-edit-input ts-box-sizing required" MaxLength="250" runat="server" />
                            </div>
                            <br />
                            <br />
                        </div>
                        <div>
                            <label class="t-label t-label-is-required"><%=Resources.Messages.EventStreet%></label>
                            <br />
                            <div class="base-edit ts-box-sizing edit base-edit-error">
                                <asp:TextBox ID="txtStreet" CssClass="base-edit-input ts-box-sizing required" MaxLength="500" runat="server" />
                            </div>
                            <br />
                            <br />
                        </div>
                        <div>
                            <label class="t-label t-label-is-required"><%=Resources.Messages.EventCity%></label>
                            <br />
                            <div class="base-edit ts-box-sizing edit base-edit-error">
                                <asp:TextBox ID="txtCity" CssClass="base-edit-input ts-box-sizing required" MaxLength="500" runat="server" />
                            </div>
                            <br />
                            <br />
                        </div>                        
                        <div>
                            <label class="t-label t-label-is-required"><%=Resources.Messages.EventState%></label>
                            <br />
                            <div class="select-option select-option-error">
                                <asp:DropDownList ID="cbState" CssClass="select-option-detail required " Width="50%" runat="server"></asp:DropDownList>
                            </div>
                            <br />
                            <% if (!HttpContext.Current.Request.UserAgent.Contains("Edge"))
                                { %>
                            <br />
                            <% } %>
                        </div>
                        <div>
                            <label class="t-label t-label-is-required"><%=Resources.Messages.Zip%></label>
                            <br />
                            <div class="base-edit ts-box-sizing edit base-edit-error">
                                <asp:TextBox ID="txtZip" CssClass="base-edit-input ts-box-sizing required" MaxLength="5" Min="1" Max="99999" runat="server" />
                            </div>
                            <br />
                            <br />
                        </div>
                        <div>
                            <label class="t-label t-label-is-required"><%=Resources.Messages.AccountPhone%></label>
                            <br />
                            <div class="base-edit ts-box-sizing edit base-edit-error">
                                <asp:TextBox ID="txtAccountPhone" CssClass="base-edit-input ts-box-sizing required" MaxLength="12" runat="server" />
                            </div>
                            <br />
                            <br />
                        </div>
                        <div>
                            <label class="t-label t-label-is-required"><%=Resources.Messages.AccountCategory%></label>
                            <br />
                            <div class="select-option select-option-error">
                                <asp:DropDownList ID="cbAccountCategory" CssClass="select-option-detail required" Width="50%" runat="server"></asp:DropDownList>
                            </div>
                            <br />
                            <% if (!HttpContext.Current.Request.UserAgent.Contains("Edge"))
                                { %>
                            <br />
                            <% } %>
                        </div>
                        <div id="divAccountCategoryOthers">
                            <label class="t-label t-label-is-required"><%=Resources.Messages.AccountCategoryOthers%></label>
                            <br />
                            <div class="base-edit ts-box-sizing edit base-edit-error">
                                <asp:TextBox ID="txtAccountCategoryOthers" CssClass="base-edit-input ts-box-sizing" MaxLength="250" runat="server" />
                            </div>
                            <br />
                            <br />
                        </div>
                        <div>
                            <label class="t-label"><%=Resources.Messages.AboutTheAccount%></label>
                            <br />
                            <div class="base-edit ts-box-sizing edit base-edit-error">
                                <asp:TextBox ID="txtAboutTheAccount" CssClass="base-edit-input ts-box-sizing" TextMode="MultiLine" Rows="6" MaxLength="250" runat="server" />
                            </div>
                            <br />
                            <br />
                        </div>
                        <div>
                            <label class="t-label t-label-is-required"><%=Resources.Messages.EventDescription%></label>
                            <br />
                            <div class="base-edit ts-box-sizing edit base-edit-error">
                                <asp:TextBox ID="txtEventDescription" CssClass="base-edit-input ts-box-sizing required" MaxLength="500" runat="server" />
                            </div>
                            <br />
                            <br />
                        </div>
                        <div>
                            <label class="t-label t-label-is-required"><%=Resources.Messages.EventDate%></label>
                            <br />
                            <div class="base-edit ts-box-sizing edit base-edit-error" id="basediveventdate">
                                <input id="txtEventDate" class="base-edit-input ts-box-sizing required" readonly runat="server">
                            </div>
                            <br />
                            <br />
                        </div>
                        <div>
                            <label class="t-label"><%=Resources.Messages.EventTime%></label>
                            <br />
                            <div class="base-edit ts-box-sizing edit base-edit-error" id="basediveventtime">
                               <!-- <input id="EventTime" class="base-edit-input ts-box-sizing" readonly runat="server">-->
                                <input type="text" id="EventTime1" name="timepicker-12-hr-clearable" class="timepicker-12-hr-clearable hasWickedpicker base-edit-input ts-box-sizing" onkeypress="return false;" style ="width: 100% !important;" tabindex="-1" readonly runat="server">                                
                             </div>                            
                            <br />
                            <br />
                        </div>
                        <div>
                            <label class="t-label t-label-is-required"><%=Resources.Messages.HostEvent%></label>
                            <br />
                            <div class="select-option selecot-option-error">
                                <asp:DropDownList ID="cbHostEvent" CssClass="select-option-detail" Width="50%" runat="server"></asp:DropDownList>
                            </div>
                            <br />
                            <% if (!HttpContext.Current.Request.UserAgent.Contains("Edge"))
                                { %>
                            <br />
                            <% } %>
                        </div>









                        <div id="Div_PreferredRestaurant">
                            <label class="t-label t-label-is-required"><%=Resources.Messages.PreferredRestaurant%></label>
                            <br />
                            <div class="select-option select-option-error">
                                <asp:DropDownList ID="cbPreferredRestaurant" CssClass="select-option-detail" Width="50%" runat="server"></asp:DropDownList>
                            </div>
                            <br />
                          
                        </div>













                        <%-- Event Address --%>
                        <div id="Div_HostEvent">
                            <div>
                                <label class="t-label t-label-is-required"><%=Resources.Messages.EventStreet%></label>
                                <br />
                                <div class="base-edit ts-box-sizing edit base-edit-error">
                                    <asp:TextBox ID="txtEventStreet" CssClass="base-edit-input ts-box-sizing" MaxLength="500" runat="server" />
                                </div>
                                <br />
                                <br />
                            </div>
                            <div>
                                <label class="t-label t-label-is-required"><%=Resources.Messages.EventCity%></label>
                                <br />
                                <div class="base-edit ts-box-sizing edit base-edit-error">
                                    <asp:TextBox ID="txtEventCity" CssClass="base-edit-input ts-box-sizing" MaxLength="500" runat="server" />
                                </div>
                                <br />
                                <br />
                            </div>
                            <div>
                                <label class="t-label t-label-is-required"><%=Resources.Messages.EventState%></label>
                                <br />
                                <div class="base-edit ts-box-sizing edit base-edit-error">
                                    <asp:TextBox ID="txtEventState" CssClass="base-edit-input ts-box-sizing" MaxLength="500" runat="server" />
                                </div>
                                <br />
                                <br />
                            </div>                        
                            <div>
                                <label class="t-label t-label-is-required"><%=Resources.Messages.EventZip%></label>
                                <br />
                                <div class="base-edit ts-box-sizing edit base-edit-error">
                                    <asp:TextBox ID="txtEventZip" CssClass="base-edit-input ts-box-sizing required" MaxLength="5" Min="1" Max="99999" runat="server" />
                                </div>
                                <br />
                                <br />
                            </div>
                        </div>
                        

                        <%--<div style="display:none">
                            <div style="padding-top: 10px; font-size: 14px">
                                <label class="t-label"><%=Resources.Messages.EventAddress%></label><br />
                                <input type="radio" name="EventOtherAddress" value="Yes">
                                Yes &nbsp;&nbsp;
                                <input type="radio" name="EventOtherAddress" checked="checked" value="No">
                                No 
                            </div>
                            <br />
                        </div>
                        <div class="clsOtherAddress">
                            <div style="padding-top: 10px; font-size: 14px">
                                <label class="t-label"><%=Resources.Messages.EventOtherAddress%></label><br />
                                <input type="radio" id="RdOtherAddressAtRestaurant" name="EventOtherAddressAtRestaurant" value="restaurant" runat="server">
                                At Restaurant &nbsp;&nbsp;
                                <input type="radio" id="RdOtherAddress" name="EventOtherAddressAtRestaurant" value="other" runat="server">
                                Other Address                                
                            </div>
                            <br />
                        </div> --%>                       
                        <%--<div style="display:none">
                            <div style="padding-top: 10px; font-size: 14px">
                                <label class="t-label"><%=Resources.Messages.UsualRestaurant%></label><br />
                                <input type="radio" name="RdUsualRestaurant" value="Yes">
                                Yes &nbsp;&nbsp;
                                <input type="radio" name="RdUsualRestaurant" checked="checked" value="No">
                                No
                            </div>
                            <br />
                        </div>--%>
                        <%--<div id="divPreferredRestaurant">
                            <label class="t-label"><%=Resources.Messages.PreferredRestaurant%> </label>
                            <br />
                            <br />
                            
                            <label class="t-label t-label-is-required"><%=Resources.Messages.EventState%></label>
                            <br />
                            <div class="select-option select-option-error">
                                <asp:DropDownList ID="cbOtherRestaurant" Width="50%" CssClass="select-option-detail " runat="server"></asp:DropDownList>
                            </div>
                            <br />
                            <% if (!HttpContext.Current.Request.UserAgent.Contains("Edge"))
                                { %>
                            <br />
                            <% } %>
                            
                            <label class="t-label t-label-is-required"><%=Resources.Messages.City%></label>
                            <br />
                            <div class="select-option select-option-error">
                                <asp:DropDownList ID="cbCity" Width="50%" CssClass="select-option-detail " runat="server"></asp:DropDownList>
                            </div>
                            <br />
                            <% if (!HttpContext.Current.Request.UserAgent.Contains("Edge"))
                                { %>
                            <br />
                            <% } %>
                            
                            <label class="t-label t-label-is-required"><%=Resources.Messages.Restaurant%></label>
                            <br />
                            <div class="select-option select-option-error">
                                <asp:DropDownList ID="cbPreferredRestaurant" Width="50%" CssClass="select-option-detail" runat="server"></asp:DropDownList>
                            </div>
                            <input runat="server" id="InpPreferredRestaurant" type="hidden" />
                            <br />
                            <% if (!HttpContext.Current.Request.UserAgent.Contains("Edge"))
                                { %>
                            <br />
                            <% } %>
                        </div>--%>
                        <div>
                            <div style="padding-top: 10px">
                                <label class="t-label"><%=Resources.Messages.SupportedEvent%></label><br />
                                <input type="radio" name="rdSupportEvent" value="Yes">
                                Yes &nbsp;&nbsp;
                                <input type="radio" name="rdSupportEvent" checked="checked" value="No">
                                No
                            </div>
                            <br />
                        </div>
                        <div id="divHowSupportedEvent">
                            <label class="t-label t-label-is-required"><%=Resources.Messages.HowSupportedEvent%></label>
                            <br />
                            <div class="base-edit ts-box-sizing edit base-edit-error">
                                <asp:TextBox ID="txtHowSupportedEvent" CssClass="base-edit-input ts-box-sizing" Rows="8" TextMode="MultiLine" MaxLength="500" runat="server" />
                            </div>
                            <br />
                            <br />
                        </div>

                        <div>
                            <label class="t-label t-label-is-required"><%=Resources.Messages.PurposeEvent%></label>
                            <br />
                            <div class="base-edit ts-box-sizing edit base-edit-error">
                                <asp:TextBox ID="txtPurposeEvent" CssClass="base-edit-input ts-box-sizing required" Rows="8" TextMode="MultiLine" MaxLength="500" runat="server" />
                            </div>
                            <br />
                            <br />
                        </div>

                        <div>
                            <label class="t-label"><%=Resources.Messages.PeopleEvent%></label>
                            <br />
                            <div class="base-edit ts-box-sizing edit base-edit-error" id="basedivpeopleevent">
                                <asp:TextBox ID="txtPeopleEvent" CssClass="base-edit-input ts-box-sizing" MaxLength="250" runat="server" />
                            </div>
                            <br />
                            <br />
                        </div>
                        <div>
                            <label class="t-label"><%=Resources.Messages.IdeasRCEvent%></label>
                            <br />
                            <div class="base-edit ts-box-sizing edit base-edit-error">
                                <asp:TextBox ID="txtIdeasRCEvent" CssClass="base-edit-input ts-box-sizing" Rows="8" TextMode="MultiLine" MaxLength="500" runat="server" />
                            </div>
                            <br />
                            <br />
                        </div>

                        <div>
                            <label class="t-label"><%=Resources.Messages.AdditionalInformationEvent%></label>
                            <br />
                            <div class="base-edit ts-box-sizing edit base-edit-error">
                                <asp:TextBox ID="txtAdditionalInformationEvent" CssClass="base-edit-input ts-box-sizing" Rows="8" TextMode="MultiLine" MaxLength="500" runat="server" />
                            </div>
                            <br />
                            <br />
                        </div>

                        <div>
                            <label class="t-label"><%=Resources.Messages.UploadAdditionalInfEvent%></label>
                            <br />
                            <div class="file-upload">
                                <div class="file-select">
                                    <div class="file-select-button" id="fileNameAdditionalInf">Choose File</div>
                                    <div class="file-select-name" id="noFileAdditionalInf">No file chosen...</div>
                                    <input type="file" name="chooseFileAdditionalInf" id="chooseFileAdditionalInf">
                                </div>
                            </div>
                            <br />
                        </div>

                        <div>
                            <label class="t-label"><%=Resources.Messages.UploadW9Event%></label>
                            <br />
                            <div class="file-upload">
                                <div class="file-select">
                                    <div class="file-select-button" id="fileNameW9">Choose File</div>
                                    <div class="file-select-name" id="noFileW9">No file chosen...</div>
                                    <input type="file" name="chooseFileW9" id="chooseFileW9">
                                </div>
                            </div>
                            <br />
                        </div>
                        <br />

                        <input id="website" name="website" type="text" value=""><!-- Leave for security protection, read docs for details -->

                    </div>
                    <div id="bottom-wizard">
                        <asp:Button ID="btnRegister" runat="server" class="button button-wrapper button-text button-style" OnClientClick="return ValidateFRM();" OnClick="btnRegister_Click" Text="<%$ Resources:Messages,BtnRegister %>" />
                    </div>
                    <br />
                </div>
            </form>

        </div>

        <div id="error" runat="server" visible="false" class="container">
            <table style="width: 100%">
                <tr>
                    <td></td>
                    <td>
                        <asp:Image ID="Image1" runat="server" ImageUrl="theme/img/logo.png" Style="float: right; width: 30%; height: auto;" /></td>
                </tr>
                <tr>
                    <td colspan="2">
                        <h3 class="h3Title"><%=Resources.Messages.MSGNOOK%></h3>
                    </td>
                </tr>
            </table>
        </div>

        <div id="divLoaderPage" runat="server" class="contentLoader">
            <div class="loader"></div>
            <div style="padding-top: 10px">
                <h3 class="h3Title"><%=Resources.Messages.Loading%></h3>
            </div>
        </div>

        <div id="divLoader" runat="server" class="contentLoader">
            <div class="loader"></div>
            <div style="padding-top: 10px">
                <h3 class="h3Title"><%=Resources.Messages.MSGSavingData%></h3>
            </div>
        </div>

        <div id="toTop"><%=Resources.Messages.BtnGoTo%></div>

        <%-- Modal Ok --%>
        <div id="ModalOk" class="modal">
            <div class="modal-content">
                <h1 class="title" style="border: none !important">THANK YOU!!</h1>
                <div id="Dv_Ok" class="container">
                    <table style="width: 100%">
                        <tr>
                            <td></td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <p><%=Resources.Messages.MSGOK%></p>
                            </td>
                        </tr>
                    </table>
                    <br />
                    <button onclick="location.href='http://www.raisingcanes.com';" class="button button-wrapper button-text button-style" style="width: 230px">Back to main page</button>
                </div>
            </div>
        </div>

        <%-- Modal Error --%>
        <div id="ModalError" class="modal">
            <div class="modal-content">
                <h1 class="title" style="border: none !important">Oops.</h1>
                <div id="Dv_Error" class="container">
                    <table style="width: 100%">
                        <tr>
                            <td></td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <p><%=Resources.Messages.MSGNOOK%></p>
                            </td>
                        </tr>
                    </table>
                    <br />
                    <button onclick="$('#ModalError').modal('close');" class="button button-wrapper button-text button-style" style="width: 130px">Close</button>
                </div>
            </div>
        </div>

        <%--<div class="container" id="footercontainer">
            <br />
            <br />
            <br />
            <br />
            <br />
            <h1>Page Footer</h1>
            <br />
            <br />
            <br />
            <br />
            <br />
        </div>--%>
    </div>

    <script src="theme/js/jquery.validate.js"></script>


</body>
</html>
