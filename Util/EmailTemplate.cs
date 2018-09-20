using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.Util
{
    public static class EmailTemplate
    {
        public const string EMAIL_TEMPLATE = @"
<!doctype html>
<html>
  <head>
    <meta name=""viewport"" content=""width=device-width"">
    <meta http-equiv=""Content-Type"" content=""text/html; charset=UTF-8"">
    <title>Auctus Experts Notification</title>
    <style>
	.logo { 
		background-size: 105px;
		width: 105px;
		height: 28px;
		background-image: url('https://dl.auctus.org/img/logo_black.png');
	}
	.social-media {
		background-size: 25px;
		width: 25px;
		height: 25px;
		display: table-cell;
		cursor: pointer;
	}
	.twitter { 
		background-image: url('https://dl.auctus.org/img/Twitter_Icon.png');
	}
	.facebook { 
		background-image: url('https://dl.auctus.org/img/Facebook_Icon.png');
	}
	.telegram { 
		width: 24px;
		background-image: url('https://dl.auctus.org/img/Telegram_Icon.png');
	}
	.separator {
		width: 11px;
		display: table-cell;
	}
    @media only screen and (max-width: 620px) {
      table[class=body] h1 {
        font-size: 28px !important;
        margin-bottom: 10px !important;
      }
      table[class=body] p,
            table[class=body] ul,
            table[class=body] ol,
            table[class=body] td,
            table[class=body] span,
            table[class=body] a {
        font-size: 16px !important;
      }
      table[class=body] .wrapper,
            table[class=body] .article {
        padding: 10px !important;
      }
      table[class=body] .content {
        padding: 0 !important;
      }
      table[class=body] .container {
        padding: 0 !important;
        width: 100% !important;
      }
      table[class=body] .main {
        border-left-width: 0 !important;
        border-radius: 0 !important;
        border-right-width: 0 !important;
      }
      table[class=body] .btn table {
        width: 100% !important;
      }
      table[class=body] .btn a {
        width: 100% !important;
      }
      table[class=body] .img-responsive {
        height: auto !important;
        max-width: 100% !important;
        width: auto !important;
      }
    }
    @media all {
      .ExternalClass {
        width: 100%;
      }
      .ExternalClass,
            .ExternalClass p,
            .ExternalClass span,
            .ExternalClass font,
            .ExternalClass td,
            .ExternalClass div {
        line-height: 100%;
      }
      .apple-link a {
        color: inherit !important;
        font-family: inherit !important;
        font-size: inherit !important;
        font-weight: inherit !important;
        line-height: inherit !important;
        text-decoration: none !important;
      }
      .btn-primary table td:hover {
        background-color: #34495e !important;
      }
      .btn-primary a:hover {
        background-color: #34495e !important;
        border-color: #34495e !important;
      }
    }
    </style>
  </head>
  <body class="""" style=""background-color: #ffffff; font-family: sans-serif; -webkit-font-smoothing: antialiased; font-size: 14px; line-height: 1.4; margin: 0; padding: 0; -ms-text-size-adjust: 100%; -webkit-text-size-adjust: 100%;"">
    <table border=""0"" cellpadding=""0"" cellspacing=""0"" class=""body"" style=""border-collapse: separate; mso-table-lspace: 0pt; mso-table-rspace: 0pt; width: 100%; background-color: #ffffff;"">
      <tr>
        <td style=""font-family: sans-serif; font-size: 14px; vertical-align: top;"">&nbsp;</td>
        <td class=""container"" style=""font-family: sans-serif; font-size: 14px; vertical-align: top; display: block; Margin: 0 auto; max-width: 600px; padding: 10px; width: 600px;"">
          <div class=""content"" style=""box-sizing: border-box; display: block; Margin: 0 auto; max-width: 600px; padding: 10px;"">

            <span class=""preheader"" style=""color: transparent; display: none; height: 0; max-height: 0; max-width: 0; opacity: 0; overflow: hidden; mso-hide: all; visibility: hidden; width: 0;"">@subject</span>
            <table class=""main"" style=""border-collapse: separate; mso-table-lspace: 0pt; mso-table-rspace: 0pt; width: 100%; background: #f7f7f7; border-radius: 3px;"">

              <tr>
                <td class=""wrapper"" style=""font-family: sans-serif; font-size: 14px; vertical-align: top; box-sizing: border-box;"">
                  <table border=""0"" cellpadding=""0"" cellspacing=""0"" style=""border-collapse: separate; mso-table-lspace: 0pt; mso-table-rspace: 0pt; width: 100%;"">
					<tr style=""width:100%;text-align:center;height:50px;vertical-align:middle;"">
						<td colspan=4>@subject</td>
					</tr>
					<tr style=""width:100%;background-color:#5a5a5a;height:200px;vertical-align:middle;"">
						<td align=""center"" colspan=4><div class=""logo"" /></td>
					</tr>
                    <tr>
                      <td style=""font-family: sans-serif; font-size: 16px; vertical-align: top;padding:50px;"" colspan=4>
                        <p style=""font-family: sans-serif; font-size: 16px; font-weight: normal; margin: 0; Margin-bottom: 25px;"">Dear user,</p>
                        @content
                        <p style=""font-family: sans-serif; font-size: 16px;color: #126efd; font-weight: normal; margin: 0; margin-top: 22px;"">Thanks,<br /><strong>Auctus Team</strong></p>
                      </td>
					</tr>
                    <tr>
					  <td style=""padding: 0px 50px;"" colspan=4>
						<div style=""height: 1px;background-color: #363636;""></div>
					  </td>
                    </tr>
					<tr style=""width:100%;height:54px;vertical-align:middle;"">
						<td style=""padding-left: 50px;""><div class=""logo"" /></td>
						<td align=""right"" style=""padding-right: 50px;"">
							<a href=""https://twitter.com/AuctusProject"" target=""_blank"" class=""social-media twitter""></a><div class=""separator""></div>
							<a href=""https://www.facebook.com/AuctusProject"" target=""_blank"" class=""social-media facebook""></a><div class=""separator""></div>
							<a href=""https://t.me/AuctusProject"" target=""_blank"" class=""social-media telegram""></a>
						</td>
					</tr>
					<tr>
					  <td style=""padding: 0px 50px;"" colspan=4>
						<div style=""height: 1px;background-color: #363636;""></div>
					  </td>
                    </tr>
					<tr>
					  <td colspan=4 class=""content-block"" style=""font-family: sans-serif; vertical-align: top; padding-bottom: 10px; padding-top: 10px; font-size: 11px; color: #212121; text-align: center;"">
						<span class=""apple-link"" style=""color: #212121; font-size: 11px; text-align: center;"">All rights reserved. Copyrights © 2018</span>
					  </td>
					</tr>
                  </table>
                </td>
              </tr>
		  </table>

          </div>
        </td>
        <td style=""font-family: sans-serif; font-size: 14px; vertical-align: top;"">&nbsp;</td>
      </tr>
    </table>
  </body>
</html>";
    }
}
