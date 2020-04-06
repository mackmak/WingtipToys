using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WingtipToys.Logic;

namespace WingtipToys
{
    public partial class ErrorPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var generalErrorMsg = "A problem has occurred on this web site. Please try again. " +
          "If this error continues, please contact support.";
            var http404ErrorMsg = "An HTTP error occurred. Page Not found. Please try again.";
            var unhandledErrorMsg = "The error was unhandled by application code.";
            const int notFound = 404;

            FriendlyErrorMsg.Text = generalErrorMsg;

            var errorHandler = Request.QueryString["handler"];
            if (errorHandler == null)
            {
                errorHandler = "Error Page";
            }

            var ex = Server.GetLastError();

            var errorCode = Request.QueryString["msg"];

            if (errorCode == "404")
            {
                ex = new HttpException(notFound, http404ErrorMsg, ex);
                FriendlyErrorMsg.Text = ex.Message;
            }

            if (ex == null)
            {
                ex = new Exception(unhandledErrorMsg);
            }

            //Showing error to developer. LOCAL REQUEST ONLY
            if (Request.IsLocal)
            {
                DetailedErrorPanel.Visible = true;

                ErrorDetailedMsg.Text = ex.Message;
                ErrorHandler.Text = errorHandler;
                if (ex.InnerException != null)
                {
                    InnerMessage.Text = ex.GetType().ToString() + "<br/>" +
                        ex.InnerException.Message;
                    InnerTrace.Text = ex.InnerException.StackTrace;
                }
                else
                {
                    InnerMessage.Text = ex.GetType().ToString();
                    if (ex.StackTrace != null)
                    {
                        InnerTrace.Text = ex.StackTrace.ToString().TrimStart();
                    }
                }
            }

            Logging.LogException(ex, errorHandler);

            Server.ClearError();
        }
    }
}