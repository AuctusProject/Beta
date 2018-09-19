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
		background-image: url('data:image/png;base64,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');
	}
	.social-media {
		background-size: 25px;
		width: 25px;
		height: 25px;
		display: table-cell;
		cursor: pointer;
	}
	.twitter { 
		background-image: url('data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABkAAAAZCAYAAADE6YVjAAAAGXRFWHRTb2Z0d2FyZQBBZG9iZSBJbWFnZVJlYWR5ccllPAAAAyhpVFh0WE1MOmNvbS5hZG9iZS54bXAAAAAAADw/eHBhY2tldCBiZWdpbj0i77u/IiBpZD0iVzVNME1wQ2VoaUh6cmVTek5UY3prYzlkIj8+IDx4OnhtcG1ldGEgeG1sbnM6eD0iYWRvYmU6bnM6bWV0YS8iIHg6eG1wdGs9IkFkb2JlIFhNUCBDb3JlIDUuNi1jMDY3IDc5LjE1Nzc0NywgMjAxNS8wMy8zMC0yMzo0MDo0MiAgICAgICAgIj4gPHJkZjpSREYgeG1sbnM6cmRmPSJodHRwOi8vd3d3LnczLm9yZy8xOTk5LzAyLzIyLXJkZi1zeW50YXgtbnMjIj4gPHJkZjpEZXNjcmlwdGlvbiByZGY6YWJvdXQ9IiIgeG1sbnM6eG1wPSJodHRwOi8vbnMuYWRvYmUuY29tL3hhcC8xLjAvIiB4bWxuczp4bXBNTT0iaHR0cDovL25zLmFkb2JlLmNvbS94YXAvMS4wL21tLyIgeG1sbnM6c3RSZWY9Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC9zVHlwZS9SZXNvdXJjZVJlZiMiIHhtcDpDcmVhdG9yVG9vbD0iQWRvYmUgUGhvdG9zaG9wIENDIDIwMTUgKE1hY2ludG9zaCkiIHhtcE1NOkluc3RhbmNlSUQ9InhtcC5paWQ6NTAwM0M2MTJCNDY3MTFFOEJGRTg4QzE3OTAzQjM1QUEiIHhtcE1NOkRvY3VtZW50SUQ9InhtcC5kaWQ6NTAwM0M2MTNCNDY3MTFFOEJGRTg4QzE3OTAzQjM1QUEiPiA8eG1wTU06RGVyaXZlZEZyb20gc3RSZWY6aW5zdGFuY2VJRD0ieG1wLmlpZDozRkNGOTRFQkI0NjcxMUU4QkZFODhDMTc5MDNCMzVBQSIgc3RSZWY6ZG9jdW1lbnRJRD0ieG1wLmRpZDozRkNGOTRFQ0I0NjcxMUU4QkZFODhDMTc5MDNCMzVBQSIvPiA8L3JkZjpEZXNjcmlwdGlvbj4gPC9yZGY6UkRGPiA8L3g6eG1wbWV0YT4gPD94cGFja2V0IGVuZD0iciI/PvswTi8AAALrSURBVHjavJbNaxpBFMBHrR+QaqMimItto9CSEGxsSw/xYDGBiIVc6l0PNgbaBIrgH1GTU4XSCoEQhNxbkIZQPCptwQq5eUqlYkysX5c09PXN8lY2m/WrKX3wY3Z3Zt7HzJs3y9hg0SPLyBukiPxEgNoifV+mcWPLNeQZ8h3ZR9aQe8gk9U/S+xr1/0BeIrpRDdwhLz8gsyPOmaXxfN7dYYMfkler7O+Ez6sij/oN4B7UaI2vIsukpxeRilot8hlJIW8BoIytGfk9hnI10lCpVE7az+fIfeSXaCSOPEaC/AWNdLCZ6Ket0+mwTCbDKpUKm5ubYysrK+z09JSdnZ11HQ7HdRrG9+gTkuQvBtqH3iYXCoXjeDwO3W4X5HJycgJLS0tAqSywuLgIfr8fdnZ2jiW+zJBerp89pTTsyd7eXp1PDofDglKpJJPJCwZENBoNpFKpuixorjfEH7Yp33tSKpXqFotFmOxyuWBzcxOKxSI0Gg2IRCKKRkKhELTbbbmRNdLPvtHB6kmtVqtHo1HATewpMRqNwrI4nU5FI1tbWzxQuRGut8SoRExKew4ODupTU1MXjAwjnU4rGbnB9avFB2mP2+1mHo+HabXakXLXarUKWaYgTdIvPFyIhHtULpfB5/ONFEUwGITz8/N+kTR5JEfILbkLZrOZYQoPjUKv17ONjQ2G2aXUfZuK7OXs4h5xt/L5PMRiMbDZbH2jSCQS0gzvm10h+TnhqVitVgGzDLLZLExPT19SrtPpYH19XVymfkY+iufk0onP5XLHgUBA0XNMBlhYWIDd3V1QEPmJ5xXZIK1dPuSJkAnNZgeXauLw8JBhRKzVajG1Ws3sdjubn59nXq+XmUwmpT3oYtqLtes99xd5Ja/Cr5F3/6AKR5EXYhX+L/eJXB5QSl/lZjyiG3agOJAvtKaj3vEzNP4rcnOcv5WY5G+FP7vFEkGtm77v07hVmje2GOi+2aZq3aRUbtL7NvUbBin5I8AACXamIWIRTLcAAAAASUVORK5CYII=');
	}
	.facebook { 
		background-image: url('data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABkAAAAZCAYAAADE6YVjAAAAGXRFWHRTb2Z0d2FyZQBBZG9iZSBJbWFnZVJlYWR5ccllPAAAAyhpVFh0WE1MOmNvbS5hZG9iZS54bXAAAAAAADw/eHBhY2tldCBiZWdpbj0i77u/IiBpZD0iVzVNME1wQ2VoaUh6cmVTek5UY3prYzlkIj8+IDx4OnhtcG1ldGEgeG1sbnM6eD0iYWRvYmU6bnM6bWV0YS8iIHg6eG1wdGs9IkFkb2JlIFhNUCBDb3JlIDUuNi1jMDY3IDc5LjE1Nzc0NywgMjAxNS8wMy8zMC0yMzo0MDo0MiAgICAgICAgIj4gPHJkZjpSREYgeG1sbnM6cmRmPSJodHRwOi8vd3d3LnczLm9yZy8xOTk5LzAyLzIyLXJkZi1zeW50YXgtbnMjIj4gPHJkZjpEZXNjcmlwdGlvbiByZGY6YWJvdXQ9IiIgeG1sbnM6eG1wPSJodHRwOi8vbnMuYWRvYmUuY29tL3hhcC8xLjAvIiB4bWxuczp4bXBNTT0iaHR0cDovL25zLmFkb2JlLmNvbS94YXAvMS4wL21tLyIgeG1sbnM6c3RSZWY9Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC9zVHlwZS9SZXNvdXJjZVJlZiMiIHhtcDpDcmVhdG9yVG9vbD0iQWRvYmUgUGhvdG9zaG9wIENDIDIwMTUgKE1hY2ludG9zaCkiIHhtcE1NOkluc3RhbmNlSUQ9InhtcC5paWQ6M0ZDRjk0RTVCNDY3MTFFOEJGRTg4QzE3OTAzQjM1QUEiIHhtcE1NOkRvY3VtZW50SUQ9InhtcC5kaWQ6M0ZDRjk0RTZCNDY3MTFFOEJGRTg4QzE3OTAzQjM1QUEiPiA8eG1wTU06RGVyaXZlZEZyb20gc3RSZWY6aW5zdGFuY2VJRD0ieG1wLmlpZDozRkNGOTRFM0I0NjcxMUU4QkZFODhDMTc5MDNCMzVBQSIgc3RSZWY6ZG9jdW1lbnRJRD0ieG1wLmRpZDozRkNGOTRFNEI0NjcxMUU4QkZFODhDMTc5MDNCMzVBQSIvPiA8L3JkZjpEZXNjcmlwdGlvbj4gPC9yZGY6UkRGPiA8L3g6eG1wbWV0YT4gPD94cGFja2V0IGVuZD0iciI/PuXODsUAAAHOSURBVHjarJY9SwNBEIYvwY+ARVJaKTYqiRgLRVSwsQnamlawyFcnov/DSuxS2FslIEYQLCwUJTHmB0gUxcrYWWh8J7zKond7c4kDTy53O/PO7s7t7jmO3QZBChyAGngFbV5rfJ6iX2DrA1nwACqgAGZAjO0x3hfY/gS2wYA2wQR7WQYJZUyC/hI36ec8x17lnO5M4p7BvJeD9OCFc9yLpajzZ0T9HGpWISJ1qIMWeAOPLj6ic0vdH9vhnGoStF1wszJ1OxZhHfyKPAQ+DOFrcA6OPfzj1BV9Z52voZ8tGgnqyvqIbjqMnzVwpAgw5/5emUR0V0Ms0AaoejhucUWPGa+2JNkHEv8O9jxiZcEeOtwiYpbetBV4WVT0Q3QKWRw/fdolPuzT3nnfbSOZAtNg0+h5lc+SbLeNRPSdO86dn40bSc6UhRfdhgzzCiwoAoZ/HQEaE91L+ZNWrpNlYyQXyiQn1Fev+KBJ4tyRI+beVfrnJKK367YLZyxBK0aShk+CjNsurDlPlri1C6fdnCffNguaPZ6MTZ6wVhvhNl4KcMbH6X8DRoN8reSNr5U8V3fUWMlJPq/QL8e4wBbheVNkIVssfIv3RbZHbCJfAgwABtGD7pNvW94AAAAASUVORK5CYII=');
	}
	.telegram { 
		background-image: url('data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABkAAAAZCAYAAADE6YVjAAAAGXRFWHRTb2Z0d2FyZQBBZG9iZSBJbWFnZVJlYWR5ccllPAAAAyhpVFh0WE1MOmNvbS5hZG9iZS54bXAAAAAAADw/eHBhY2tldCBiZWdpbj0i77u/IiBpZD0iVzVNME1wQ2VoaUh6cmVTek5UY3prYzlkIj8+IDx4OnhtcG1ldGEgeG1sbnM6eD0iYWRvYmU6bnM6bWV0YS8iIHg6eG1wdGs9IkFkb2JlIFhNUCBDb3JlIDUuNi1jMDY3IDc5LjE1Nzc0NywgMjAxNS8wMy8zMC0yMzo0MDo0MiAgICAgICAgIj4gPHJkZjpSREYgeG1sbnM6cmRmPSJodHRwOi8vd3d3LnczLm9yZy8xOTk5LzAyLzIyLXJkZi1zeW50YXgtbnMjIj4gPHJkZjpEZXNjcmlwdGlvbiByZGY6YWJvdXQ9IiIgeG1sbnM6eG1wPSJodHRwOi8vbnMuYWRvYmUuY29tL3hhcC8xLjAvIiB4bWxuczp4bXBNTT0iaHR0cDovL25zLmFkb2JlLmNvbS94YXAvMS4wL21tLyIgeG1sbnM6c3RSZWY9Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC9zVHlwZS9SZXNvdXJjZVJlZiMiIHhtcDpDcmVhdG9yVG9vbD0iQWRvYmUgUGhvdG9zaG9wIENDIDIwMTUgKE1hY2ludG9zaCkiIHhtcE1NOkluc3RhbmNlSUQ9InhtcC5paWQ6M0ZDRjk0RTlCNDY3MTFFOEJGRTg4QzE3OTAzQjM1QUEiIHhtcE1NOkRvY3VtZW50SUQ9InhtcC5kaWQ6M0ZDRjk0RUFCNDY3MTFFOEJGRTg4QzE3OTAzQjM1QUEiPiA8eG1wTU06RGVyaXZlZEZyb20gc3RSZWY6aW5zdGFuY2VJRD0ieG1wLmlpZDozRkNGOTRFN0I0NjcxMUU4QkZFODhDMTc5MDNCMzVBQSIgc3RSZWY6ZG9jdW1lbnRJRD0ieG1wLmRpZDozRkNGOTRFOEI0NjcxMUU4QkZFODhDMTc5MDNCMzVBQSIvPiA8L3JkZjpEZXNjcmlwdGlvbj4gPC9yZGY6UkRGPiA8L3g6eG1wbWV0YT4gPD94cGFja2V0IGVuZD0iciI/Pqti7N0AAAH+SURBVHjavJY7SwNBFIUX8RGwMAiCD/BVqCRIUigi+AOiiI36AxSSmE5ECzut7LRSLIRUamNpQIxgJ/jEGAuxkwQMgmK0UpB4bjgryzq72cTggY/szsw9M3PnsdE0e9WAANgECfAKcvxNsDzAdkWrEoRAGsRBBPiBm/VuvkdY/wjmQLXTDro5yhjwOozxsr3E9RRq3M9RhbXSJHEZMGDVQEbwxBz/RQH6/JpRFaca0soj8bmh74/mmdNSNAS2wK2pPEbfvFxcB28Rxo1gETxwS+e4E43y0Ff8tQluQycaBbsG4zvwyecNRXvxnZSHKPe7lbrAErg3mEu+R8AO+GDZlCI2Qv98gN9U2QGmwYHBWCfCFOyZynsVnYhvUuMV4TZVHivMT0AL65OmuhdQoeikjv75RmY1gXHDaFdZXg/eFQOwW9O8f1YxE6PWwTCoBV+KDoQVi1iZSVammALtNp1sM7fLFikRXViUy9qmJegcDNp0ImvRCWb5fqRoc20RK75nGvdxoXOyxrSM8V3S98yyN5u4Q/2cODnxzYr6BnBqMTP9xGf0E6/fXftaeSV+C6pbOFimDoKqW/hfvie6+ril//JlTPELa6tWcMmcOr3+PWx/BdqK+bcyY/i3Is8+nmD9JPtYHme7MOOKlovfmygXMsuzkeV7lPUuO5NvAQYAOxaOMZEKu1sAAAAASUVORK5CYII=');
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

            <span class=""preheader"" style=""color: transparent; display: none; height: 0; max-height: 0; max-width: 0; opacity: 0; overflow: hidden; mso-hide: all; visibility: hidden; width: 0;"">This is preheader text. Some clients will show this text as a preview.</span>
            <table class=""main"" style=""border-collapse: separate; mso-table-lspace: 0pt; mso-table-rspace: 0pt; width: 100%; background: #f7f7f7; border-radius: 3px;"">

              <tr>
                <td class=""wrapper"" style=""font-family: sans-serif; font-size: 14px; vertical-align: top; box-sizing: border-box;"">
                  <table border=""0"" cellpadding=""0"" cellspacing=""0"" style=""border-collapse: separate; mso-table-lspace: 0pt; mso-table-rspace: 0pt; width: 100%;"">
					<tr style=""width:100%;text-align:center;height:50px;vertical-align:middle;"">
						<td colspan=4>Verify your email address - Auctus Experts</td>
					</tr>
					<tr style=""width:100%;background-color:#5a5a5a;height:200px;vertical-align:middle;"">
						<td align=""center"" colspan=4><div class=""logo"" /></td>
					</tr>
                    <tr>
                      <td style=""font-family: sans-serif; font-size: 16px; vertical-align: top;padding:50px;"" colspan=4>
                        <p style=""font-family: sans-serif; font-size: 16px; font-weight: normal; margin: 0; Margin-bottom: 25px;"">Dear user,</p>
                        <p style=""font-family: sans-serif; font-size: 16px; font-weight: normal; margin: 0; Margin-bottom: 48px;"">To activate your Auctus Expert account please <a href=""#"">click here</a>.</p>
                        <p style=""font-family: sans-serif; font-size: 12px; font-style: italic; margin: 0; Margin-bottom: 22px;"">If you didn’t ask to verify this address, you can ignore this email.</p>
                        <p style=""font-family: sans-serif; font-size: 16px;color: #126efd; font-weight: normal; margin: 0;"">Thanks,<br /><strong>Auctus Team</strong></p>
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
							<a href=""https://twitter.com/AuctusProject"" target=""_blank"" class=""social-media twitter""></a>
							<a href=""https://www.facebook.com/AuctusProject"" target=""_blank"" class=""social-media facebook""></a>
							<a href=""https://t.me/AuctusProject"" target=""_blank"" class=""social-media telegram""></a>
						</td>
					</tr>
					<tr>
					  <td style=""padding: 0px 50px;"" colspan=4>
						<div style=""height: 1px;background-color: #363636;""></div>
					  </td>
                    </tr>
					<tr>
					  <td class=""content-block"" style=""font-family: sans-serif; vertical-align: top; padding-bottom: 10px; padding-top: 10px; font-size: 11px; color: #212121; text-align: center;"">
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
