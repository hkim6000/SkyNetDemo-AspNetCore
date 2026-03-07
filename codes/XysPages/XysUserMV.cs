using SkyNet;
using SkyNet.ToolKit;
using ASPNETCoreWeb.codes.XysBases;

namespace ASPNETCoreWeb.codes.XysPages
{

    public class XysUserMV : WebBase
    {
        private SQLGridSection.SQLGridInfo SQLGridInfo;

        public XysUserMV() {
            SQLGridInfo = new SQLGridSection.SQLGridInfo
            {
                Id = "DataGrid",
                Name = "DataGrid",
                CurrentPageNo = (int)(Common.Val(ParamValue("DataGrid_PageNo")) == 0 ? 1 : Common.Val(ParamValue("DataGrid_PageNo"))),
                LinesPerPage = 50,
                ExcludeDownloadColumns = new int[] { 0 },
                TDictionary = HtmlTranslator.TDictionary,
                Query = new SQLGridSection.SQLQuery
                {
                    Tables = "XysUser a inner join XysUserInfo b on a.UserId = b.UserId ",
                    OrderBy = new string[] { "b.UserName" },
                    Columns = new string[] { "a.UserId", "b.UserName", "b.UserPhone", "b.UserEmail", "a.UserOTP", "dbo.XF_LevelName(a.LevelCode) Level", "a.UserStatus" },
                    ColumnAlias = new string[] {
                        "",
                        Translator.Format("username"),
                        Translator.Format("userphone"),
                        Translator.Format("useremail"),
                        Translator.Format("userotp"),
                        Translator.Format("userlevel"),
                        Translator.Format("userstatus")
                    },
                    Filters = string.IsNullOrEmpty(ParamValue("DataGrid_Filter")) ? string.Empty : ParamValue("DataGrid_Filter")
                }
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
            filterBtn.SetAttribute(HtmlEvents.onclick, ByPassCall("XysUserMV/SearchClicked"));

            FilterSection filter = new();
            filter.ModalWrap = true;
            filter.Wrap.SetStyle(HtmlStyles.marginTop, "110px");
            filter.Wrap.SetStyle(HtmlStyles.width, "90%");
            filter.Menu = ViewMenuItems;
            filter.FilterHtml = filterText.HtmlText + filterBtn.HtmlText;

            SQLGridSection SQLGrid = new SQLGridSection(SQLGridInfo);
            if (SQLGrid.Grid != null) SQLGrid.Grid.Table.SetAttribute(HtmlAttributes.@class, "tableX");
            SetGridStyle(SQLGrid);

            HtmlElementBox elmBox = new HtmlElementBox();
            elmBox.SetAttribute(HtmlAttributes.id, References.Elements.ElmBox);
            elmBox.SetStyle(HtmlStyles.width, "90%");
            elmBox.SetStyle(HtmlStyles.margin, "auto");
            elmBox.SetStyle(HtmlStyles.marginTop, "8px");
            elmBox.SetStyle(HtmlStyles.marginBottom, "80px");
            elmBox.Wrap.SetStyle(HtmlStyles.margin, "20px 10px 20px 10px");

            elmBox.AddItem(SQLGrid);

            return filter.HtmlText + elmBox.HtmlText;
        }

        private void SetGridStyle(SQLGridSection SQLGrid)
        {
            SQLGrid.Wrap.SetStyle(HtmlStyles.margin, string.Empty);
            SQLGrid.Wrap.SetStyle(HtmlStyles.marginLeft, "8px");
            SQLGrid.Wrap.SetStyle(HtmlStyles.display, "inline-block");

            if (SQLGrid.GridData != null)
            {
                ViewMethod editMethod = GetViewMethod("edit");

                SQLGrid.Grid.TableColumns[0].SetHeaderStyle(HtmlStyles.display, "none");
                SQLGrid.Grid.TableColumns[0].SetColumnStyle(HtmlStyles.display, "none");

                SQLGrid.Grid.TableColumns[1].SetColumnAttribute(HtmlEvents.onclick, ByPassCall(editMethod.Method, editMethod.Params));
                SQLGrid.Grid.TableColumns[1].SetColumnStyle(HtmlStyles.textDecoration, "underline");
                SQLGrid.Grid.TableColumns[1].SetColumnStyle(HtmlStyles.cursor, "pointer");
                SQLGrid.Grid.TableColumns[1].SetColumnStyle(HtmlStyles.whiteSpace, "nowrap");

                SQLGrid.Grid.TableColumns[4].SetColumnFormat("@R {4} | 0. , 1.✓");
                SQLGrid.Grid.TableColumns[6].SetColumnFormat("@R {6} | 0.Normal, 8.Suspended, 9.Terminated");

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

            SQLGridInfo.Query.Filters = "b.UserName + b.UserEmail+ b.UserPhone like N'%" + FilterBoxValue + "%' ";
            SQLGridSection SQLGrid = new SQLGridSection(SQLGridInfo);
            if (SQLGrid.Grid != null) SQLGrid.Grid.Table.SetAttribute(HtmlAttributes.@class, "tableX");
            SetGridStyle(SQLGrid);

            ApiResponse _ApiResponse = new ApiResponse();
            _ApiResponse.ReplaceSQLGridSection("DataGrid", SQLGrid);
            _ApiResponse.StoreLocalValue("FilterBoxValue", FilterBoxValue);
            return _ApiResponse;
        }
    }

}
