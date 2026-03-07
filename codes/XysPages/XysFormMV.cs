using SkyNet;
using SkyNet.ToolKit;
using ASPNETCoreWeb.codes.XysBases;
using static SkyNet.ToolKit.SQLGridSection;
using static SkyNet.WebPage;

namespace ASPNETCoreWeb.codes.XysPages
{


    public class XysFormMV : WebBase
    {
        private SQLGridSection.SQLGridInfo SQLGridInfo;
        public XysFormMV()
        {
            SQLGridInfo = new SQLGridSection.SQLGridInfo();
            SQLGridInfo.TDictionary = this.HtmlTranslator.TDictionary;
            SQLGridInfo.Name = Translator.Format("title");
            SQLGridInfo.Id = this.GetType().Name;

            double pageNo = Common.Val(ParamValue("DataGrid_PageNo"));
            SQLGridInfo.CurrentPageNo = (int)(pageNo == 0 ? 1 : pageNo);

            SQLGridInfo.LinesPerPage = 50;
            SQLGridInfo.ExcludeDownloadColumns = new int[] { 0 };
            SQLGridInfo.DisplayCount = SQLGridSection.DisplayCounts.FilteredNAll;
            SQLGridInfo.TitleEnabled = true;

            SQLGridInfo.Query = new SQLGridSection.SQLQuery
            {
                Tables = "XysForm",
                OrderBy = new string[] { "FormTitle" },
                Columns = new string[] {
                "FormId",
                "FormTitle",
                "FormDesc",
                "FormRef",
                "FormFlag",
                "dbo.XF_OffetTime(SYSDTE," + CSTimeOffset.ToString() + ") SYSDTE",
                "dbo.XF_UserName(SYSUSR) SYSUSR"
            },
                ColumnAlias = new string[] {
                Translator.Format("formid"),
                Translator.Format("formtitle"),
                Translator.Format("formdesc"),
                Translator.Format("formref"),
                Translator.Format("formflag"),
                Translator.Format("SYSDTE"),
                Translator.Format("SYSUSR")
            },
                Filters = string.IsNullOrEmpty(ParamValue("DataGrid_Filter")) ? string.Empty : ParamValue("DataGrid_Filter")
            };
        }

        public override string InitialViewHtml()
        {
            MenuList ViewMenuItems = GetViewMenuItems();
            Wrap ViewButtons = GetViewButtons(new string[] { });

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
            filterBtn.SetAttribute(HtmlEvents.onclick, ByPassCall("XysFormMV/SearchClicked"));

            FilterSection filter = new FilterSection();
            filter.ModalWrap = true;
            filter.Wrap.SetStyle(HtmlStyles.marginTop, "110px");
            filter.Wrap.SetStyle(HtmlStyles.width, "92%");
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
            elmBox.SetStyle(HtmlStyles.width, "92%");
            elmBox.SetStyle(HtmlStyles.margin, "auto");
            elmBox.SetStyle(HtmlStyles.marginTop, "8px");
            elmBox.SetStyle(HtmlStyles.marginBottom, "80px");
            elmBox.Wrap.SetStyle(HtmlStyles.margin, "20px 10px 20px 10px");

            elmBox.AddItem(SQLGrid);

            string ViewHtml = filter.HtmlText + elmBox.HtmlText;

            return ViewHtml;
        }

        private void SetGridStyle(SQLGridSection SQLGrid)
        {
            SQLGrid.Wrap.SetStyle(HtmlStyles.margin, string.Empty);
            SQLGrid.Wrap.SetStyle(HtmlStyles.marginLeft, "8px");
            SQLGrid.Wrap.SetStyle(HtmlStyles.display, "inline-block");
            SQLGrid.Wrap.SetStyle(HtmlStyles.width, "100%");
            SQLGrid.Wrap.SetStyle(HtmlStyles.overflow, "auto");

            if (SQLGrid.GridData != null)
            {
                ViewMethod editMethod = GetViewMethod("edit");

                SQLGrid.Grid.TableColumns[0].SetHeaderStyle(HtmlStyles.display, "none");
                SQLGrid.Grid.TableColumns[0].SetColumnStyle(HtmlStyles.display, "none");

                SQLGrid.Grid.TableColumns[1].SetColumnStyle(HtmlStyles.whiteSpace, "nowrap");
                SQLGrid.Grid.TableColumns[1].SetColumnAttribute(HtmlEvents.onclick, ByPassCall(editMethod.Method, editMethod.Params));
                SQLGrid.Grid.TableColumns[1].SetColumnStyle(HtmlStyles.textDecoration, "underline");
                SQLGrid.Grid.TableColumns[1].SetColumnStyle(HtmlStyles.cursor, "pointer");

                SQLGrid.Grid.TableColumns[4].SetColumnFormat("@R {4} | 0. , 1.✓");

                for (int i = 0; i < SQLGrid.Grid.TableColumns.Count; i++)
                {
                    switch (i)
                    {
                        case 0:
                        case 1:
                        case 2:
                        case 3:
                            SQLGrid.Grid.TableColumns[i].SetColumnStyle(HtmlStyles.textAlign, "left");
                            break;
                        default:
                            SQLGrid.Grid.TableColumns[i].SetColumnStyle(HtmlStyles.textAlign, "center");
                            break;
                    }
                }
            }
        }

        public ApiResponse SearchClicked()
        {
            string FilterBoxValue = ParamValue("FilterBox");

            SQLGridInfo.Query.Filters = "FormTitle + FormDesc + FormRef like N'%" + FilterBoxValue + "%' ";
            SQLGridSection SQLGrid = new SQLGridSection(SQLGridInfo);

            if (SQLGrid.Grid != null)
            {
                SQLGrid.Grid.Table.SetAttribute(HtmlAttributes.@class, "tableX");
            }
            SetGridStyle(SQLGrid);

            ApiResponse _ApiResponse = new ApiResponse();
            _ApiResponse.ReplaceSQLGridSection("DataGrid", SQLGrid);
            _ApiResponse.StoreLocalValue("FilterBoxValue", FilterBoxValue);
            return _ApiResponse;
        }
    }
}
