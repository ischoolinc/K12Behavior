using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace K12.Behavior.DisciplineInput
{
    public class ConfigObj
    {
        public DateTime InputStartTime;
        public DateTime InputEndTime;
        public bool AllowMeritA;
        public bool AllowMeritB;
        public bool AllowMeritC;
        public bool AllowDemeritA;
        public bool AllowDemeritB;
        public bool AllowDemeritC;
        private XElement xe;
        public ConfigObj()
        { 
            DataTable dt = tool._Q.Select("select content from list where name = 'K12.Behavior.DisciplineInput'");
            if (dt.Rows.Count > 0)
            {
                this.xe = new XElement("Detail");
                try
                {
                    string tmp = "" + dt.Rows[0]["content"];
                    this.xe = XElement.Parse(tmp);
                    DateTime.TryParse("" + xe.Element("InputStartTime").Value, out this.InputStartTime);
                    DateTime.TryParse("" + xe.Element("InputEndTime").Value, out this.InputEndTime);
                    if ( xe.Element("AllowedDiscipline") != null )
                    { 
                        XElement tmpxe ;
                        if ((tmpxe = xe.Element("AllowedDiscipline").Element("Merit")) != null)
                        {
                            this.AllowMeritA = tmpxe.Attribute("A").Value == "1" ? true : false;
                            this.AllowMeritB = tmpxe.Attribute("B").Value == "1" ? true : false;
                            this.AllowMeritC = tmpxe.Attribute("C").Value == "1" ? true : false;
                        }
                        if ((tmpxe = xe.Element("AllowedDiscipline").Element("Demerit")) != null)
                        {
                            this.AllowDemeritA = tmpxe.Attribute("A").Value == "1" ? true : false;
                            this.AllowDemeritB = tmpxe.Attribute("B").Value == "1" ? true : false;
                            this.AllowDemeritC = tmpxe.Attribute("C").Value == "1" ? true : false;
                        }
                    }
                }
                catch (Exception) { }
            }
        }
        public bool Save()
        {
            bool useInsert = false;
            if (xe == null)
            {
                xe = new XElement("Detail");
                useInsert = true;
            }
            if (xe.Element("InputStartTime") == null)
                xe.Add(new XElement("InputStartTime"));
            xe.Element("InputStartTime").Value = "" + this.InputStartTime.ToString("yyyy/MM/dd HH:mm");
            if (xe.Element("InputEndTime") == null)
                xe.Add(new XElement("InputEndTime"));
            xe.Element("InputEndTime").Value = "" + this.InputEndTime.ToString("yyyy/MM/dd HH:mm");
            if (xe.Element("AllowedDiscipline") == null)
                xe.Add(new XElement("AllowedDiscipline"));
            if (xe.Element("AllowedDiscipline").Element("Merit") == null)
                xe.Element("AllowedDiscipline").Add(new XElement("Merit"));
            if (xe.Element("AllowedDiscipline").Element("Demerit") == null)
                xe.Element("AllowedDiscipline").Add(new XElement("Demerit"));
            xe.Element("AllowedDiscipline").Element("Merit").SetAttributeValue("A", this.AllowMeritA ? 1 : 0);
            xe.Element("AllowedDiscipline").Element("Merit").SetAttributeValue("B", this.AllowMeritB ? 1 : 0);
            xe.Element("AllowedDiscipline").Element("Merit").SetAttributeValue("C", this.AllowMeritC ? 1 : 0);
            xe.Element("AllowedDiscipline").Element("Demerit").SetAttributeValue("A", this.AllowDemeritA ? 1 : 0);
            xe.Element("AllowedDiscipline").Element("Demerit").SetAttributeValue("B", this.AllowDemeritB ? 1 : 0);
            xe.Element("AllowedDiscipline").Element("Demerit").SetAttributeValue("C", this.AllowDemeritC ? 1 : 0);

            string sql = "";
            if ( useInsert )
                sql = "insert into list (name,content) values ('K12.Behavior.DisciplineInput','" + xe.ToString(SaveOptions.DisableFormatting) + "')";
            else 
                sql = "update list set content = '" + xe.ToString(SaveOptions.DisableFormatting) + "' where name = 'K12.Behavior.DisciplineInput'";
            try
            {
                int result = tool._U.Execute(sql);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
