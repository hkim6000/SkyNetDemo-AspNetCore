using SkyNet;
using SkyNet.ToolKit;
using ASPNETCoreWeb.codes.XysBases;
using static SkyNet.WebPage;

namespace ASPNETCoreWeb.codes.XysPages
{

    public class XysSignupExpired : WebSingle
    {
        public override void OnInitialized()
        {
            HtmlDoc.AddJsFile("WebScript.js");
            HtmlDoc.AddCSSFile("WebStyle.css");
            HtmlDoc.SetTitle(Translator.Format("title"));

            Label Title = new Label(Translator.Format("expired"));
            Title.Wrap.SetStyle(HtmlStyles.marginTop, "50px");
            Title.Wrap.SetStyle(HtmlStyles.marginLeft, "6px");
            Title.Wrap.SetStyle(HtmlStyles.fontSize, "24px");
            Title.Wrap.SetStyle(HtmlStyles.textShadow, "2px 2px #e0e0e0");

            Label lbl1 = new Label(Translator.Format("pinexpired"));
            lbl1.Wrap.SetStyle(HtmlStyles.paddingLeft, "10px");
            lbl1.Wrap.SetStyle(HtmlStyles.color, "#444");

            Button btn = new Button(Translator.Format("back"), Button.ButtonTypes.Button);
            btn.SetStyle(HtmlStyles.marginLeft, "6px");
            btn.SetAttribute(HtmlAttributes.@class, "button");
            btn.SetAttribute(HtmlEvents.onclick, "NavXysSignup()");

            HtmlElementBox elmBox = new HtmlElementBox();
            elmBox.SetAttribute(HtmlAttributes.id, References.Elements.ElmBox);
            elmBox.SetStyle(HtmlStyles.position, "relative");
            elmBox.SetStyle(HtmlStyles.margin, "auto");
            elmBox.SetStyle(HtmlStyles.width, "500px");
            elmBox.SetStyle(HtmlStyles.paddingLeft, "50px");
            elmBox.SetStyle(HtmlStyles.paddingBottom, "24px");

            elmBox.AddItem(Title, 30);
            elmBox.AddItem(lbl1, 16);
            elmBox.AddItem(btn, 10);

            HtmlDoc.HtmlBodyAddOn = elmBox.HtmlText;

            HtmlDoc.InitialScripts.CenteringElement(References.Elements.ElmBox);
        }

        public ApiResponse NavXysSignup()
        {
            ApiResponse _ApiResponse = new ApiResponse();
            _ApiResponse.Navigate(References.Pages.XysSignup);
            return _ApiResponse;
        }
    }

}
