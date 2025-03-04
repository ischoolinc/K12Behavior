using System.Xml;

namespace K12.Behavior.Feature
{
    public class PeriodInfo
    {
        public PeriodInfo()
        {
        }

        public PeriodInfo(XmlElement element)
        {
            _name = element.GetAttribute("Name");
            _type = element.GetAttribute("Type");
            _course_period = element.GetAttribute("CoursePeriod");
            string s = element.GetAttribute("Sort");
            if (!int.TryParse(s, out _sort))
                _sort = int.MaxValue;
            _aggregated = element.GetAttribute("Aggregated");
        }

        private string _name;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private int _sort;

        public int Sort
        {
            get { return _sort; }
            set { _sort = value; }
        }

        //2024/10/8 - New
        private string _course_period;

        public string CoursePeriod
        {
            get { return _course_period; }
            set { _course_period = value; }
        }

        private string _type;

        public string Type
        {
            get { return _type; }
            set { _type = value; }
        }

        private string _aggregated;
        public string Aggregated
        {
            get { return _aggregated; }
        }
    }
}