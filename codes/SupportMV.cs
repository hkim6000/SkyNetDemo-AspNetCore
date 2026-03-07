using SkyNet;
using SkyNet.ToolKit;
using ASPNETCoreWeb.codes.XysBases;

namespace ASPNETCoreWeb.codes
{

    public class SupportMV : WebBase
    {
        public override string InitialViewHtml()
        {
            MenuList ViewMenuItems = GetViewMenuItems();
            Wrap ViewButtons = GetViewButtons(new string[] { });

            Label label = new Label();
            label.Wrap.SetStyles("font-weight:700;font-size:22px;margin:12px;");
            label.Wrap.InnerText = Translator.Format("support");

            FilterSection filter = new();
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
            elmBox.Wrap.SetStyle(HtmlStyles.margin, "20px 10px 20px 10px");

            string ViewHtml = filter.HtmlText + elmBox.HtmlText;

            return ViewHtml;
        }
    }

}
