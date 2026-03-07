using SkyNet;
using SkyNet.ToolKit;

namespace ASPNETCoreWeb.codes.XysBases
{
    public class WebGridEV : WebBase
    {
        protected string MyPageType = string.Empty;

        public override string InitialViewHtml()
        {
            MenuList ViewMenuItems = GetViewMenuItems();
            Wrap ViewButtons = GetViewButtons(ViewPart.Data == null ? new string[] { "save" } : null);

            Label label = new Label();
            label.Wrap.SetStyles("font-weight:700;font-size:22px;margin:12px;");
            label.Wrap.InnerText = ViewPart.UIControl.UIMode == UIModes.New ? Translator.Format("new") : Translator.Format("edit");

            FilterSection filter = new FilterSection();
            filter.ModalWrap = true;
            filter.Wrap.SetStyle(HtmlStyles.marginTop, "110px");
            filter.Wrap.SetStyle(HtmlStyles.width, "90%");
            filter.Menu = ViewMenuItems;
            filter.FilterHtml = label.HtmlText;

            HtmlElementBox elmBox = new HtmlElementBox();
            elmBox.SetAttribute(HtmlAttributes.id, References.Elements.ElmBox);
            elmBox.SetStyle(HtmlStyles.width, "90%");
            elmBox.SetStyle(HtmlStyles.margin, "auto");
            elmBox.SetStyle(HtmlStyles.marginTop, "8px");
            elmBox.SetStyle(HtmlStyles.marginBottom, "80px");
            elmBox.Wrap.SetStyle(HtmlStyles.margin, "50px 30px 30px 50px");

            elmBox.AddItem(ViewPart.UIControl, 20);
            elmBox.AddItem(ViewButtons, 20);

            string ViewHtml = filter.HtmlText + elmBox.HtmlText;
            return ViewHtml;
        }

        public ApiResponse SaveData()
        {
            ApiResponse _ApiResponse = new ApiResponse();

            string rlt = VerifySave();
            if (rlt != string.Empty)
            {
                _ApiResponse.PopUpWindow(DialogMsgRequred(rlt), References.Elements.PageContents);
            }
            else
            {
                rlt = PutSaveData();
                if (rlt == string.Empty)
                {
                    _ApiResponse.PopUpWindow(DialogMsgSaved("m=" + MyPageType + "MV"), References.Elements.PageContents);
                }
                else
                {
                    _ApiResponse.PopUpWindow(DialogMsg(rlt), References.Elements.PageContents);
                }
            }

            return _ApiResponse;
        }

        protected virtual string PutSaveData()
        {
            return string.Empty;
        }

        protected virtual string VerifySave()
        {
            return string.Empty;
        }

        public ApiResponse DeleteData()
        {
            ApiResponse _ApiResponse = new ApiResponse();
            _ApiResponse.PopUpWindow(DialogQstDelete(MyPageType + "EV/ConfirmDeleteData"), References.Elements.PageContents);
            return _ApiResponse;
        }

        public ApiResponse ConfirmDeleteData()
        {
            ApiResponse _ApiResponse = new ApiResponse();

            string rlt = VerifyDelete();
            if (rlt != string.Empty)
            {
                _ApiResponse.PopUpWindow(DialogMsgRequred(rlt), References.Elements.PageContents);
            }
            else
            {
                rlt = PutDeleteData();
                if (rlt == string.Empty)
                {
                    _ApiResponse.PopUpWindow(DialogMsgDeleted("m=" + MyPageType + "MV"), References.Elements.PageContents);
                }
                else
                {
                    _ApiResponse.PopUpWindow(DialogMsg(rlt), References.Elements.PageContents);
                }
            }

            return _ApiResponse;
        }

        protected virtual string PutDeleteData()
        {
            return string.Empty;
        }

        protected virtual string VerifyDelete()
        {
            return string.Empty;
        }
    }

}
