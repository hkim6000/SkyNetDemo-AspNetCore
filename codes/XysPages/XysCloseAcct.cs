using Microsoft.Data.SqlClient;
using SkyNet;
using SkyNet.ToolKit;
using ASPNETCoreWeb.codes.XysBases;
using System.Data;

namespace ASPNETCoreWeb.codes.XysPages
{
    public class XysCloseAcct : WebBase
    {
        public override string InitialViewHtml()
        {
            MenuList ViewMenuItems = GetViewMenuItems();
            Wrap ViewButtons = GetViewButtons(new string[] { });

            Label label = new Label();
            label.Wrap.SetStyles("font-weight:700;font-size:22px;margin:12px;");
            label.Wrap.InnerText = Translator.Format("cancel");

            FilterSection filter = new();
            filter.ModalWrap = true;
            filter.Wrap.SetStyle(HtmlStyles.marginTop, "100px");
            filter.Wrap.SetStyle(HtmlStyles.width, "95%");
            filter.Menu = ViewMenuItems;
            filter.FilterHtml = label.HtmlText;

            Label label1 = new Label();
            label1.Wrap.SetStyles("font-size:18px;");
            label1.Wrap.InnerText = Translator.Format("losedata");

            HtmlElementBox elmBox = new HtmlElementBox();
            elmBox.SetAttribute(HtmlAttributes.id, References.Elements.ElmBox);
            elmBox.SetStyle(HtmlStyles.width, "95%");
            elmBox.SetStyle(HtmlStyles.margin, "auto");
            elmBox.SetStyle(HtmlStyles.marginTop, "10px");
            elmBox.SetStyle(HtmlStyles.marginBottom, "80px");
            elmBox.Wrap.SetStyle(HtmlStyles.margin, "40px 10px 40px 40px");

            elmBox.AddItem(label1, 50);
            elmBox.AddItem(GetViewButtons());

            string ViewHtml = filter.HtmlText + elmBox.HtmlText;
            return ViewHtml;
        }

        public ApiResponse CancelView()
        {
            string t = GetDataValue("t");

            ApiResponse _ApiResponse = new ApiResponse();
            DialogBox dialogBox = new DialogBox(Translator.Format("closeaccount"));
            dialogBox.ContentsWrap.SetStyles("padding:30px;width:300px;height:60px;text-align:center;");
            dialogBox.AddButton(Translator.Format("yes"), string.Empty, "class:button1;onclick:" + ByPassCall("XysCloseAcct/CancelViewConfirm"));
            dialogBox.AddButton(Translator.Format("no"), string.Empty, "onclick:$PopOff();class:button;");
            _ApiResponse.PopUpWindow(dialogBox.HtmlText, References.Elements.PageContents);
            return _ApiResponse;
        }

        public ApiResponse CancelViewConfirm()
        {
            ApiResponse _ApiResponse = new ApiResponse();
            string rlt = PutCancelViewData();
            if (rlt == string.Empty)
            {
                _ApiResponse.ExecuteScript("SignOut()");
            }
            else
            {
                _ApiResponse.PopUpWindow(DialogMsg(rlt), References.Elements.PageContents);
            }
            return _ApiResponse;
        }

        private string PutCancelViewData()
        {
            List<string> SQL = new List<string>
            {
                " delete from Membership where UserId = @UserId ",
                " delete from XysUser where UserId = @UserId ",
                " delete from XysUserInfo where UserId = @UserId ",
                " delete from XysUserReset where UserId = @UserId "
            };

            List<SqlParameter> SqlParams = new List<SqlParameter>();
            SqlParams.Add(new SqlParameter { ParameterName = "@UserId", Value = AppKey.UserId, SqlDbType = SqlDbType.NVarChar });

            return PutData(SqlWithParams(SQL, SqlParams));
        }
    }

}
