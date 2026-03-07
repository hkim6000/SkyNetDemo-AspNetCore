using SkyNet;
using SkyNet.ToolKit;
using ASPNETCoreWeb.codes.XysBases;

namespace ASPNETCoreWeb.codes.XysPages
{

    public class XysUnAuthorized : WebSingle
    {
        public override void OnInitialized()
        {
            HtmlDoc.AddJsFile("WebScript.js");
            HtmlDoc.AddCSSFile("WebStyle.css");
            HtmlDoc.SetTitle(Translator.Format("title"));

            Label Title = new Label(Translator.Format("unauthorize"));
            Title.Wrap.SetStyle(HtmlStyles.marginTop, "40px");
            Title.Wrap.SetStyle(HtmlStyles.marginLeft, "16px");
            Title.Wrap.SetStyle(HtmlStyles.fontSize, "20px");
            Title.Wrap.SetStyle(HtmlStyles.textShadow, "2px 2px #e0e0e0");

            Button btn = new Button(Translator.Format("home"), Button.ButtonTypes.Button);
            btn.SetAttribute(HtmlAttributes.@class, "button");
            btn.SetStyle(HtmlStyles.marginLeft, "20px");
            btn.SetAttribute(HtmlEvents.onclick, ByPassCall("XysUnAuthorized/Navigate", "m=" + References.Pages.XysHome));

            HtmlElementBox elmBox = new HtmlElementBox();
            elmBox.SetAttribute(HtmlAttributes.id, References.Elements.ElmBox);
            elmBox.SetStyle(HtmlStyles.width, "90%");
            elmBox.SetStyle(HtmlStyles.margin, "auto");
            elmBox.SetStyle(HtmlStyles.marginTop, "108px");
            elmBox.SetStyle(HtmlStyles.marginBottom, "80px");
            elmBox.Wrap.SetStyle(HtmlStyles.margin, "20px 10px 20px 10px");

            elmBox.AddItem(Title, 30);
            elmBox.AddItem(btn, 1);

            HtmlDoc.HtmlBodyAddOn = elmBox.HtmlText;
        }

        public ApiResponse Navigate()
        {
            string m = GetDataValue("m");
            ApiResponse _ApiResponse = new ApiResponse();
            _ApiResponse.Navigate(m);
            return _ApiResponse;
        }
    }

}
