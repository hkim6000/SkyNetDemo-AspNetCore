using SkyNet;
using SkyNet.ToolKit;
using ASPNETCoreWeb.codes.XysBases;
using static SkyNet.WebPage;

namespace ASPNETCoreWeb.codes.XysPages
{

    public class XysRoleMV : WebBase
    {
        public override void InitialViewData()
        {
            string sSql = " Select RoleId,RoleName,RoleAlias,RoleOrder From XysRole order by RoleOrder,RoleName";
            ViewPart.BindData(sSql);
        }

        public override string InitialViewHtml()
        {
            MenuList ViewMenuItems = GetViewMenuItems();
            Wrap ViewButtons = GetViewButtons(ViewPart.Data == null ? new string[] { "save" } : null);

            Label label = new Label();
            label.Wrap.SetStyles("font-weight:700;font-size:22px;margin:12px;");
            label.Wrap.InnerText = ViewPart.Data == null ? Translator.Format("role") : Translator.Format("role");

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
            elmBox.Wrap.SetStyle(HtmlStyles.margin, "30px 30px 30px 30px");

            if (ViewPart.Data != null)
            {
                List<ViewModel> data = DataTableListT<ViewModel>(ViewPart.Data);
                for (int i = 0; i < data.Count; i++)
                {
                    HtmlTag elm = new HtmlTag(HtmlTags.img, HtmlTag.Types.Empty);
                    elm.SetAttribute(HtmlAttributes.title, data[i].RoleName);
                    elm.SetAttribute(HtmlAttributes.src, ImagePath + "role.jpg");
                    elm.SetStyle(HtmlStyles.width, "60px");

                    HtmlTag elm1 = new HtmlTag();
                    elm1.SetStyle(HtmlStyles.padding, "6px");
                    elm1.InnerText = data[i].RoleName + "<br>(" + data[i].RoleAlias + ")";

                    ViewMethod editMethod = GetViewMethod("edit");

                    ItemPanel itmPnl = new ItemPanel();
                    itmPnl.Wrap.SetAttribute(HtmlEvents.onclick, ByPassCall(editMethod.Method, string.Format(editMethod.Params, data[i].RoleId)));
                    itmPnl.Wrap.SetAttribute(HtmlAttributes.id, data[i].RoleId);
                    itmPnl.Wrap.SetAttribute(HtmlAttributes.@class, "itmPnl");
                    itmPnl.Wrap.SetStyle(HtmlStyles.boxShadow, "3px 4px 6px 1px rgba(0, 0, 0, 0.15)");
                    itmPnl.Wrap.SetStyle(HtmlStyles.minWidth, "100px");
                    itmPnl.AddElement(elm, HorizontalAligns.Center);
                    itmPnl.AddElement(elm1, HorizontalAligns.Center);
                    elmBox.AddItem(itmPnl);
                }
            }

            return filter.HtmlText + elmBox.HtmlText;
        }

        private class ViewModel
        {
            public string RoleId { get; set; } = string.Empty;
            public string RoleName { get; set; } = string.Empty;
            public string RoleAlias { get; set; } = string.Empty;
            public string RoleOrder { get; set; } = string.Empty;
        }
    }

}
