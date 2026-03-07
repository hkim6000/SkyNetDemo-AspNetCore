using SkyNet;
using SkyNet.ToolKit;
using System.Collections;
using System.Data.SqlTypes;
using static SkyNet.ToolKit.SQLGridSection;
using static System.Net.WebRequestMethods;

namespace ASPNETCoreWeb.codes.XysBases
{
    public class WebGridMV : WebBase
    {
        protected string SQLGridPage = string.Empty;
        protected string SQLGridFilter = string.Empty;
        protected string[] SQLGridOrderBy = new string[0];

        protected SQLGridSection.SQLGridInfo SQLGridInfo = new();

        public WebGridMV() {
            SQLGridInfo.TDictionary = this.HtmlTranslator.TDictionary;
        }

        public override string InitialViewHtml()
        {
            MenuList ViewMenuItems = GetViewMenuItems();
            Wrap ViewButtons = GetViewButtons(new string[0]);

            Texts filterText = new Texts(TextTypes.text);
            filterText.Wrap.SetStyle(HtmlStyles.margin, "2px");
            filterText.Wrap.SetStyle(HtmlStyles.marginTop, "4px");
            filterText.Wrap.SetStyle(HtmlStyles.marginLeft, "8px");
            filterText.Text.SetStyle(HtmlStyles.fontSize, "16px");
            filterText.Text.SetStyle(HtmlStyles.height, "24px");
            filterText.Text.SetAttribute(HtmlAttributes.placeholder, Translator.Format("searchterm"));
            filterText.Text.SetAttribute(HtmlAttributes.id, "FilterBox");
            filterText.Text.SetAttribute(HtmlAttributes.value, ParamValue("FilterBoxValue"));

            Button filterBtn = new Button();
            filterBtn.SetStyle(HtmlStyles.backgroundImage, "url('" + ImagePath + "search.jpg')");
            filterBtn.SetStyle(HtmlStyles.backgroundRepeat, "no-repeat");
            filterBtn.SetStyle(HtmlStyles.backgroundSize, "24px 24px");
            filterBtn.SetStyle(HtmlStyles.borderRadius, "50%");
            filterBtn.SetStyle(HtmlStyles.border, "1px solid #ddd");
            filterBtn.SetStyle(HtmlStyles.padding, "6px");
            filterBtn.SetStyle(HtmlStyles.height, "30px");
            filterBtn.SetStyle(HtmlStyles.width, "30px");
            filterBtn.SetStyle(HtmlStyles.boxShadow, "1px 2px 2px 1px rgba(0, 0, 0, 0.15)");
            filterBtn.SetAttribute(HtmlEvents.onclick, ByPassCall(SQLGridPage + "/SearchClicked"));

            FilterSection filter = new FilterSection();
            filter.ModalWrap = true;
            filter.Wrap.SetStyle(HtmlStyles.marginTop, "110px");
            filter.Wrap.SetStyle(HtmlStyles.width, "90%");
            filter.Menu = ViewMenuItems;
            filter.FilterHtml = filterText.HtmlText + filterBtn.HtmlText;

            SQLGridSection SQLGrid = new SQLGridSection(SQLGridInfo);
            if (SQLGrid.Grid != null)
            {
                SQLGrid.Grid.Table.SetAttribute(HtmlAttributes.@class, "tableX");
            }
            SetGridStyle(SQLGrid);

            HtmlElementBox elmBox = new HtmlElementBox();
            elmBox.SetAttribute(HtmlAttributes.id, References.Elements.ElmBox);
            elmBox.SetStyle(HtmlStyles.width, "90%");
            elmBox.SetStyle(HtmlStyles.margin, "auto");
            elmBox.SetStyle(HtmlStyles.marginTop, "8px");
            elmBox.SetStyle(HtmlStyles.marginBottom, "80px");
            elmBox.SetStyle(HtmlStyles.overflow, "auto");
            elmBox.Wrap.SetStyle(HtmlStyles.margin, "20px 10px 20px 10px");

            elmBox.AddItem(SQLGrid);

            string ViewHtml = filter.HtmlText + elmBox.HtmlText;

            return ViewHtml;
        }

        protected virtual void SetGridStyle(SQLGridSection SQLGrid)
        {
        }

        public virtual ApiResponse SearchClicked()
        {
            string FilterBoxValue = ParamValue("FilterBox");

            SQLGridInfo.Query.Filters = SQLGridFilter + "  like N'%" + FilterBoxValue + "%' ";
            SQLGridSection SQLGrid = new SQLGridSection(SQLGridInfo);
            if (SQLGrid.Grid != null)
            {
                SQLGrid.Grid.Table.SetAttribute(HtmlAttributes.@class, "tableX");
            }
    
            SetGridStyle(SQLGrid);

            ApiResponse _ApiResponse = new ApiResponse();
            _ApiResponse.ReplaceSQLGridSection(SQLGridInfo.Id, SQLGrid);
            _ApiResponse.StoreLocalValue("FilterBoxValue", FilterBoxValue);
            return _ApiResponse;
        }
    }

}
