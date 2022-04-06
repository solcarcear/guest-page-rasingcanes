using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Raisingcanes.Site
{
    public partial class Result : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
          
            if ((string)Session["Result"] == "Ok")
            {
                
                this.complete.Visible = true;
                this.error.Visible = false;
            }
            else
            {   
                this.complete.Visible = false;
                this.error.Visible = true;            
            }
        }
    }
}