using SkyNet;
using SkyNet.ToolKit;
using ASPNETCoreWeb.codes.XysBases;

namespace ASPNETCoreWeb.codes.XysPages
{

    public class XysReport : WebBase
    {
        private OptionValues rpts = new OptionValues();

        public XysReport()
        {
            rpts.AddItem(Translator.Format("select"), "00000");

            List<NameValue> data = GetNameValue();
            if (data != null && data.Count > 0)
            {
                for (int i = 0; i < data.Count; i++)
                {
                    rpts.AddItem(data[i].name, data[i].value);
                }
            }
        }

        private List<NameValue> GetNameValue()
        {
            string SSQL = " select MenuDesc name,MenuTag value " +
                          " from XysMenu where MenuArea = N'X' and  " +
                          " PageId = (select PageId from XysPage where PageName = N'XysReport') order by MenuTag ";

            // Assuming DataTableListT is a helper in the base or a utility class
            List<NameValue> data = DataTableListT<NameValue>(SQLData.SQLDataTable(SSQL));
            return data;
        }

        public override string InitialViewHtml()
        {
            MenuList ViewMenuItems = GetViewMenuItems(ViewPart.Data == null ? new string[] { "xysrole" } : new string[] { });
            Wrap ViewButtons = GetViewButtons(ViewPart.Data == null ? new string[] { "save" } : null);

            Label label = new Label();
            label.Wrap.SetStyles("font-weight:700;font-size:22px;margin:12px;");
            label.Wrap.InnerText = Translator.Format("report");

            FilterSection filter = new();
            filter.ModalWrap = true;
            filter.Wrap.SetStyle(HtmlStyles.marginTop, "100px");
            filter.Wrap.SetStyle(HtmlStyles.width, "95%");
            filter.Menu = ViewMenuItems;
            filter.FilterHtml = label.HtmlText;

            Dropdown filterDD = new Dropdown();
            filterDD.Wrap.SetStyle(HtmlStyles.margin, "2px");
            filterDD.Wrap.SetStyle(HtmlStyles.marginTop, "10px");
            filterDD.SelBox.SetStyle(HtmlStyles.fontSize, "20px");
            filterDD.SelBox.SetStyle(HtmlStyles.borderColor, "#333");
            filterDD.SelBox.SetAttribute(HtmlAttributes.id, "rpt");
            filterDD.SelBox.SetAttribute(HtmlEvents.onchange, "CallReport(this)");
            filterDD.SelBox.InnerText = rpts.OptionsHtml();

            FilterSection FilterSectionObj = new FilterSection();
            FilterSectionObj.Wrap.SetStyle(HtmlStyles.marginTop, "10px");
            FilterSectionObj.Wrap.SetStyle(HtmlStyles.width, "100%");
            FilterSectionObj.FilterWrap.SetStyle(HtmlStyles.padding, string.Empty);
            FilterSectionObj.FilterHtml = filterDD.HtmlText;

            Wrap WrapObj = new Wrap();
            WrapObj.SetAttribute(HtmlAttributes.id, "RptBox");
            WrapObj.SetStyle(HtmlStyles.boxSizing, "border-box");
            WrapObj.SetStyle(HtmlStyles.padding, "8px");
            WrapObj.SetStyle(HtmlStyles.width, "100%");
            WrapObj.SetStyle(HtmlStyles.border, "1px solid #aaa");
            WrapObj.SetStyle(HtmlStyles.borderRadius, "6px");
            WrapObj.SetStyle(HtmlStyles.boxShadow, "1px 2px 2px 1px rgba(0, 0, 0, 0.15)");

            WrapObj.InnerText = Rpt_Empty();

            HtmlElementBox elmBox = new HtmlElementBox();
            elmBox.SetAttribute(HtmlAttributes.id, References.Elements.ElmBox);
            elmBox.SetStyle(HtmlStyles.width, "95%");
            elmBox.SetStyle(HtmlStyles.margin, "auto");
            elmBox.SetStyle(HtmlStyles.marginTop, "10px");
            elmBox.SetStyle(HtmlStyles.marginBottom, "80px");
            elmBox.Wrap.SetStyle(HtmlStyles.margin, "10px");

            elmBox.AddItem(FilterSectionObj, 10);
            elmBox.AddItem(WrapObj, 10);

            HtmlDoc.AddJsFile(WebEnv.HeaderScripts.UserIconScript);
            HtmlDoc.AddJsFile(WebEnv.HeaderScripts.TimerScript);
            HtmlDoc.InitialScripts.ExecuteScript("$StartTimer('tmr')");

            TitleSection2 PageLayout = PageTitle();
            PageLayout.ContentWrap.SetAttribute(HtmlAttributes.id, References.Elements.PageContents);
            PageLayout.ContentWrap.InnerText = filter.HtmlText + elmBox.HtmlText;

            return PageLayout.HtmlText;
        }

        public ApiResponse Navigate()
        {
            string m = GetDataValue("m");
            ApiResponse _ApiResponse = new ApiResponse();
            _ApiResponse.Navigate(m);
            return _ApiResponse;
        }

        private string Rpt_Empty()
        {
            Wrap WrapObj = new Wrap();
            WrapObj.SetStyle(HtmlStyles.display, "table-cell");
            WrapObj.SetStyle(HtmlStyles.verticalAlign, "bottom");
            WrapObj.SetStyle(HtmlStyles.fontSize, "20px");
            WrapObj.SetStyle(HtmlStyles.color, "darkolivegreen");
            WrapObj.InnerText = Translator.Format("selrpt");
            return WrapObj.HtmlText;
        }

        public ApiResponse CallReport()
        {
            string rpt = GetDataValue("rpt");
            ApiResponse _ApiResponse = new ApiResponse();
            string rpthtml = string.Empty;

            switch (rpt)
            {
                case "00000":
                    rpthtml = Rpt_Empty();
                    break;
                case "10000":
                    // rpthtml = Rpt_10000();
                    break;
                case "20000":
                    // rpthtml = Rpt_20000();
                    break;
                case "30000":
                    // rpthtml = Rpt_30000();
                    break;
            }

            _ApiResponse.SetElementContents("RptBox", rpthtml);
            return _ApiResponse;
        }

        private string Rpt_10000()
        {
            string rptHtml = string.Empty;
            string bdt = DateTime.Now.ToString("yyyy-MM-dd");

            Texts filterText = new Texts(TextTypes.date);
            filterText.Wrap.SetStyle(HtmlStyles.margin, "2px");
            filterText.Wrap.SetStyle(HtmlStyles.marginRight, "10px");
            filterText.Wrap.SetStyle(HtmlStyles.marginTop, "10px");
            filterText.Text.SetStyle(HtmlStyles.fontSize, "20px");
            filterText.Text.SetStyle(HtmlStyles.height, "20px");
            filterText.Text.SetStyle(HtmlStyles.padding, "8px");
            filterText.Text.SetStyle(HtmlStyles.border, "none");
            filterText.Text.SetStyle(HtmlStyles.borderBottom, "1px solid #aaa");
            filterText.Text.SetAttribute(HtmlAttributes.id, "fltdte");
            filterText.Text.SetAttribute(HtmlAttributes.value, bdt);

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
            filterBtn.SetAttribute(HtmlEvents.onclick, ByPassCall("Report/RptRlt_10000", "fltdte=::"));

            FilterSection FilterSectionObj = new FilterSection();
            FilterSectionObj.Wrap.SetStyle(HtmlStyles.marginTop, "20px");
            FilterSectionObj.Wrap.SetStyle(HtmlStyles.width, "100%");
            FilterSectionObj.Wrap.SetStyle(HtmlStyles.padding, string.Empty);
            FilterSectionObj.FilterHtml = filterText.HtmlText + filterBtn.HtmlText;

            Wrap WrapObj = new Wrap();
            WrapObj.SetAttribute(HtmlAttributes.id, "RptRlt");
            WrapObj.SetStyle(HtmlStyles.marginLeft, "8px");
            WrapObj.SetStyle(HtmlStyles.paddingTop, "8px");
            WrapObj.SetStyle(HtmlStyles.width, "100%");

            rptHtml = FilterSectionObj.HtmlText + WrapObj.HtmlText;
            return rptHtml;
        }

        public ApiResponse RptRlt_10000()
        {
            string fltdte = GetDataValue("fltdte");
            ApiResponse _ApiResponse = new ApiResponse();

            DateTime tempDate;
            if (!DateTime.TryParse(fltdte, out tempDate))
            {
                _ApiResponse.SetElementContents("RptRlt", Translator.Format("errdatatype"));
            }
            else
            {
                SQLGridSection.SQLGridInfo SQLGridInfo = new SQLGridSection.SQLGridInfo
                {
                    Id = "ClockInGrid",
                    Name = Translator.Format("clockin"),
                    CurrentPageNo = 1,
                    LinesPerPage = 50,
                    DisplayCount = SQLGridSection.DisplayCounts.FilteredOnly,
                    TitleEnabled = true,
                    TDictionary = this.HtmlTranslator.TDictionary,
                    Query = new SQLGridSection.SQLQuery
                    {
                        Tables = "TagLog a inner join Loc b on a.LocId = b.LocId inner join Mem c on a.MemId = c.MemId ",
                        OrderBy = new string[] { "a.sysdte desc" },
                        Columns = new string[] { "c.MemName", "c.MemEmail", "b.LocName", "left(a.CDST,5) Distance", "b.LocUnit",
                                             "dateadd(hour, convert(int,LocTime), a.sysdte) Local" },
                        ColumnAlias = new string[] {
                        Translator.Format("name"),
                        Translator.Format("email"),
                        Translator.Format("loc"),
                        Translator.Format("dst"),
                        Translator.Format("unit"),
                        Translator.Format("time")
                    },
                        Filters = " convert(varchar(10),dateadd(hour, convert(int,LocTime), a.sysdte),121) = '" +
                                  tempDate.ToString("yyyy-MM-dd") + "' "
                    }
                };

                SQLGridSection SQLGrid = new SQLGridSection(SQLGridInfo);
                if (SQLGrid.Grid != null) SQLGrid.Grid.Table.SetAttribute(HtmlAttributes.@class, "tableX");
                SetGridStyle(SQLGrid);

                _ApiResponse.SetElementContents("RptRlt", SQLGrid.HtmlText);
            }

            return _ApiResponse;
        }

        private void SetGridStyle(SQLGridSection SQLGrid)
        {
            SQLGrid.Wrap.SetStyle(HtmlStyles.margin, string.Empty);
            SQLGrid.Wrap.SetStyle(HtmlStyles.display, "inline-block");

            if (SQLGrid.GridData != null)
            {
                SQLGrid.Grid.TableColumns[2].SetColumnStyle(HtmlStyles.whiteSpace, "nowrap");
                SQLGrid.Grid.TableColumns[5].SetColumnStyle(HtmlStyles.whiteSpace, "nowrap");
                SQLGrid.Grid.TableColumns[4].SetColumnFormat("@R {4} | K.Km, M.Mile");
                SQLGrid.Grid.TableColumns[5].SetColumnFormat("@D {5} | MM/dd/yyyy HH:mm:ss");

                for (int i = 0; i < SQLGrid.Grid.TableColumns.Count; i++)
                {
                    switch (i)
                    {
                        case 0:
                        case 1:
                            SQLGrid.Grid.TableColumns[i].SetColumnStyle(HtmlStyles.textAlign, "left");
                            break;
                        default:
                            SQLGrid.Grid.TableColumns[i].SetColumnStyle(HtmlStyles.textAlign, "center");
                            break;
                    }
                }
            }
        }

        private string Rpt_20000()
        {
            string rptHtml = string.Empty;
            string bdt = DateTime.Now.ToString("yyyy-MM");

            Texts filterText = new Texts(TextTypes.month);
            filterText.Wrap.SetStyle(HtmlStyles.margin, "2px");
            filterText.Wrap.SetStyle(HtmlStyles.marginRight, "10px");
            filterText.Wrap.SetStyle(HtmlStyles.marginTop, "10px");
            filterText.Text.SetStyle(HtmlStyles.fontSize, "20px");
            filterText.Text.SetStyle(HtmlStyles.height, "20px");
            filterText.Text.SetStyle(HtmlStyles.padding, "8px");
            filterText.Text.SetStyle(HtmlStyles.border, "none");
            filterText.Text.SetStyle(HtmlStyles.borderBottom, "1px solid #aaa");
            filterText.Text.SetAttribute(HtmlAttributes.id, "fltdte");
            filterText.Text.SetAttribute(HtmlAttributes.value, bdt);

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
            filterBtn.SetAttribute(HtmlEvents.onclick, ByPassCall("Report/RptRlt_20000", "fltdte=::"));

            FilterSection FilterSectionObj = new FilterSection();
            FilterSectionObj.Wrap.SetStyle(HtmlStyles.marginTop, "20px");
            FilterSectionObj.Wrap.SetStyle(HtmlStyles.width, "100%");
            FilterSectionObj.Wrap.SetStyle(HtmlStyles.padding, string.Empty);
            FilterSectionObj.FilterHtml = filterText.HtmlText + filterBtn.HtmlText;

            Wrap WrapObj = new Wrap();
            WrapObj.SetAttribute(HtmlAttributes.id, "RptRlt");
            WrapObj.SetStyle(HtmlStyles.marginLeft, "8px");
            WrapObj.SetStyle(HtmlStyles.paddingTop, "8px");
            WrapObj.SetStyle(HtmlStyles.width, "100%");

            rptHtml = FilterSectionObj.HtmlText + WrapObj.HtmlText;
            return rptHtml;
        }

        public ApiResponse RptRlt_20000()
        {
            string fltdte = GetDataValue("fltdte");
            ApiResponse _ApiResponse = new ApiResponse();

            DateTime tempDate;
            if (!DateTime.TryParse(fltdte, out tempDate))
            {
                _ApiResponse.SetElementContents("RptRlt", Translator.Format("errdatatype"));
            }
            else
            {
                SQLGridSection.SQLGridInfo SQLGridInfo = new SQLGridSection.SQLGridInfo
                {
                    Id = "ClockInGrid",
                    Name = Translator.Format("clockin"),
                    CurrentPageNo = 1,
                    LinesPerPage = 50,
                    DisplayCount = SQLGridSection.DisplayCounts.FilteredOnly,
                    TitleEnabled = true,
                    TDictionary = this.HtmlTranslator.TDictionary,
                    Query = new SQLGridSection.SQLQuery
                    {
                        Tables = "TagLog a inner join Loc b on a.LocId = b.LocId inner join Mem c on a.MemId = c.MemId ",
                        OrderBy = new string[] { "a.sysdte desc" },
                        Columns = new string[] { "c.MemName", "c.MemEmail", "b.LocName", "left(a.CDST,5) Distance", "b.LocUnit",
                                             "dateadd(hour, convert(int,LocTime), a.sysdte) Local" },
                        ColumnAlias = new string[] {
                        Translator.Format("name"),
                        Translator.Format("email"),
                        Translator.Format("loc"),
                        Translator.Format("dst"),
                        Translator.Format("unit"),
                        Translator.Format("time")
                    },
                        Filters = " convert(varchar(7),dateadd(hour, convert(int,LocTime), a.sysdte),121) = '" +
                                  tempDate.ToString("yyyy-MM") + "' "
                    }
                };

                SQLGridSection SQLGrid = new SQLGridSection(SQLGridInfo);
                if (SQLGrid.Grid != null) SQLGrid.Grid.Table.SetAttribute(HtmlAttributes.@class, "tableX");
                SetGridStyle(SQLGrid);

                _ApiResponse.SetElementContents("RptRlt", SQLGrid.HtmlText);
            }

            return _ApiResponse;
        }

        private string Rpt_30000()
        {
            string rptHtml = string.Empty;
            string bdt = DateTime.Now.AddDays(-60).ToString("yyyy-MM-dd");
            string edt = DateTime.Now.ToString("yyyy-MM-dd");

            Texts filterText = new Texts(TextTypes.date);
            filterText.Wrap.SetStyle(HtmlStyles.margin, "2px");
            filterText.Wrap.SetStyle(HtmlStyles.marginRight, "10px");
            filterText.Wrap.SetStyle(HtmlStyles.marginTop, "10px");
            filterText.Text.SetStyle(HtmlStyles.fontSize, "20px");
            filterText.Text.SetStyle(HtmlStyles.height, "20px");
            filterText.Text.SetStyle(HtmlStyles.padding, "8px");
            filterText.Text.SetStyle(HtmlStyles.border, "none");
            filterText.Text.SetStyle(HtmlStyles.borderBottom, "1px solid #aaa");
            filterText.Text.SetAttribute(HtmlAttributes.id, "fltdte");
            filterText.Text.SetAttribute(HtmlAttributes.value, bdt);

            Texts filterText1 = new Texts(TextTypes.date);
            filterText1.Wrap.SetStyle(HtmlStyles.margin, "2px");
            filterText1.Wrap.SetStyle(HtmlStyles.marginRight, "10px");
            filterText1.Wrap.SetStyle(HtmlStyles.marginTop, "10px");
            filterText1.Text.SetStyle(HtmlStyles.fontSize, "20px");
            filterText1.Text.SetStyle(HtmlStyles.height, "20px");
            filterText1.Text.SetStyle(HtmlStyles.padding, "8px");
            filterText1.Text.SetStyle(HtmlStyles.border, "none");
            filterText1.Text.SetStyle(HtmlStyles.borderBottom, "1px solid #aaa");
            filterText1.Text.SetAttribute(HtmlAttributes.id, "fltdte1");
            filterText1.Text.SetAttribute(HtmlAttributes.value, edt);

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
            filterBtn.SetAttribute(HtmlEvents.onclick, ByPassCall("Report/RptRlt_30000", "fltdte=::&fltdte1=::"));

            FilterSection FilterSectionObj = new FilterSection();
            FilterSectionObj.Wrap.SetStyle(HtmlStyles.marginTop, "20px");
            FilterSectionObj.Wrap.SetStyle(HtmlStyles.width, "100%");
            FilterSectionObj.Wrap.SetStyle(HtmlStyles.padding, string.Empty);
            FilterSectionObj.FilterHtml = filterText.HtmlText + filterText1.HtmlText + filterBtn.HtmlText;

            Wrap WrapObj = new Wrap();
            WrapObj.SetAttribute(HtmlAttributes.id, "RptRlt");
            WrapObj.SetStyle(HtmlStyles.marginLeft, "8px");
            WrapObj.SetStyle(HtmlStyles.paddingTop, "8px");
            WrapObj.SetStyle(HtmlStyles.width, "100%");

            rptHtml = FilterSectionObj.HtmlText + WrapObj.HtmlText;
            return rptHtml;
        }

        public ApiResponse RptRlt_30000()
        {
            string fltdte = GetDataValue("fltdte");
            string fltdte1 = GetDataValue("fltdte1");
            ApiResponse _ApiResponse = new ApiResponse();

            DateTime date1, date2;
            if (!DateTime.TryParse(fltdte, out date1) || !DateTime.TryParse(fltdte1, out date2))
            {
                _ApiResponse.SetElementContents("RptRlt", Translator.Format("errdatatype"));
            }
            else
            {
                SQLGridSection.SQLGridInfo SQLGridInfo = new SQLGridSection.SQLGridInfo
                {
                    Id = "ClockInGrid",
                    Name = Translator.Format("clockin"),
                    CurrentPageNo = 1,
                    LinesPerPage = 50,
                    DisplayCount = SQLGridSection.DisplayCounts.FilteredOnly,
                    TitleEnabled = true,
                    TDictionary = this.HtmlTranslator.TDictionary,
                    Query = new SQLGridSection.SQLQuery
                    {
                        Tables = "TagLog a inner join Loc b on a.LocId = b.LocId inner join Mem c on a.MemId = c.MemId ",
                        OrderBy = new string[] { "a.sysdte desc" },
                        Columns = new string[] { "c.MemName", "c.MemEmail", "b.LocName", "left(a.CDST,5) Distance", "b.LocUnit",
                                             "dateadd(hour, convert(int,LocTime), a.sysdte) Local" },
                        ColumnAlias = new string[] {
                        Translator.Format("name"),
                        Translator.Format("email"),
                        Translator.Format("loc"),
                        Translator.Format("dst"),
                        Translator.Format("unit"),
                        Translator.Format("time")
                    },
                        Filters = " convert(varchar(10),dateadd(hour, convert(int,LocTime), a.sysdte),121) between " +
                                  " '" + date1.ToString("yyyy-MM-dd") + "' and '" + date2.ToString("yyyy-MM-dd") + "' "
                    }
                };

                SQLGridSection SQLGrid = new SQLGridSection(SQLGridInfo);
                if (SQLGrid.Grid != null) SQLGrid.Grid.Table.SetAttribute(HtmlAttributes.@class, "tableX");
                SetGridStyle(SQLGrid);

                _ApiResponse.SetElementContents("RptRlt", SQLGrid.HtmlText);
            }

            return _ApiResponse;
        }
    }

}
