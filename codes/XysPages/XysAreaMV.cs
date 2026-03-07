using SkyNet;
using SkyNet.ToolKit;
using ASPNETCoreWeb.codes.XysBases;

namespace ASPNETCoreWeb.codes.XysPages
{
    public class XysAreaMV : WebGridMV
    {
        public XysAreaMV()
        {
            SQLGridPage = this.GetType().Name;
            SQLGridFilter = "AreaName + CountryName + CountryAlias";
            SQLGridOrderBy = new string[] { "AreaOrder" };
            string GridName = Translator.Format("title");
            string GridTables = "XysArea";

            UIGrid UIGrid = new UIGrid();
            UIGrid.Items.AddRange(new UIGrid.Item[] {
                  new UIGrid.Item { Name = "Edit", Value = "'&#10148;'", Label = string.Empty },
                  new UIGrid.Item { Name = "AreaId", Label = "AreaId", IsKey = true },
                  new UIGrid.Item { Name = "AreaName", Label = "AreaName" },
                  new UIGrid.Item { Name = "CountryName", Label = "CountryName" },
                  new UIGrid.Item { Name = "CountryAlias", Label = "CountryAlias" },
                  new UIGrid.Item { Name = "AreaOrder", Label = "AreaOrder" },
                  new UIGrid.Item { Name = "AreaFlag", Label = "AreaFlag" },
                  new UIGrid.Item { Name = "SYSDTE", Label = "SYSDTE" },
                  new UIGrid.Item { Name = "SYSUSR", Value = "dbo.XF_UserName(SYSUSR)", Label = "SYSUSR" }
            });

            SQLGridInfo.Name = GridName; 
            SQLGridInfo.Id = this.GetType().Name;

            int pageNoVal = (int)Common.Val(ParamValue(SQLGridInfo.Id + "_PageNo"));
            SQLGridInfo.CurrentPageNo = (int)(pageNoVal == 0 ? 1 : pageNoVal);

            SQLGridInfo.LinesPerPage = 30;
            SQLGridInfo.ExcludeDownloadColumns = new int[] { 0 };
            SQLGridInfo.DisplayCount = SQLGridSection.DisplayCounts.FilteredOnly;
            SQLGridInfo.TitleEnabled = true;

            SQLGridInfo.Query = new SQLGridSection.SQLQuery
            {
                Tables = GridTables,
                OrderBy = SQLGridOrderBy,
                Columns = UIGrid.Columns().ToArray(),
                ColumnAlias = UIGrid.Labels().ToArray(),
                Filters = string.IsNullOrEmpty(ParamValue(SQLGridInfo.Id + "_Filter"))
                    ? SQLGridFilter + " like '%%' "
                    : ParamValue(SQLGridInfo.Id + "_Filter")
            };
        }

        protected override void SetGridStyle(SQLGridSection SQLGrid)
        {
            SQLGrid.Wrap.SetStyle(HtmlStyles.margin, string.Empty);
            SQLGrid.Wrap.SetStyle(HtmlStyles.marginLeft, "8px");
            SQLGrid.Wrap.SetStyle(HtmlStyles.display, "inline-block");
            SQLGrid.Wrap.SetStyle(HtmlStyles.width, "100%");
            SQLGrid.Wrap.SetStyle(HtmlStyles.overflow, "auto");

            if (SQLGrid.GridData != null)
            {
                ViewMethod editMethod = GetViewMethod("edit");

                SQLGrid.Grid.TableColumns[0].SetColumnStyle(HtmlStyles.cursor, "pointer");
                SQLGrid.Grid.TableColumns[0].SetColumnAttribute(HtmlEvents.onclick, ByPassCall(editMethod.Method, editMethod.Params));

                SQLGrid.Grid.TableColumns[1].SetColumnStyle(HtmlStyles.whiteSpace, "nowrap");
                SQLGrid.Grid.TableColumns[2].SetColumnStyle(HtmlStyles.whiteSpace, "nowrap");
                SQLGrid.Grid.TableColumns[3].SetColumnStyle(HtmlStyles.whiteSpace, "nowrap");

                SQLGrid.Grid.TableColumns[6].SetColumnFormat("@R {6} | 0. , 1.✓");
                SQLGrid.Grid.TableColumns[7].SetColumnFormat("@D {7} |" + LocalDateFormat);

                for (int i = 0; i < SQLGrid.Grid.TableColumns.Count; i++)
                {
                    switch (i)
                    {
                        case 0:
                        case 1:
                        case 4:
                        case 5:
                        case 6:
                        case 7:
                            SQLGrid.Grid.TableColumns[i].SetColumnStyle(HtmlStyles.textAlign, "center");
                            break;
                        default:
                            SQLGrid.Grid.TableColumns[i].SetColumnStyle(HtmlStyles.textAlign, "left");
                            break;
                    }
                }
            }
        }

    }
}
