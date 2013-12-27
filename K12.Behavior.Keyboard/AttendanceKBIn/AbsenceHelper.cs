using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using FISCA.DSAUtil;
using FISCA.Presentation.Controls;
using K12.Data;

namespace K12.Behavior.Keyboard
{
    class AbsenceHelper
    {
        /// <summary>
        /// 名稱,熱鍵
        /// </summary>
        private Dictionary<string, string> _GetNameHotKey = new Dictionary<string, string>();

        /// <summary>
        /// 熱鍵,名稱
        /// </summary>
        private Dictionary<string, string> _GetHotKeyName = new Dictionary<string, string>();

        /// <summary>
        /// 名稱,縮寫
        /// </summary>
        private Dictionary<string, string> _GetNameAbbreviation = new Dictionary<string, string>();

        /// <summary>
        /// 熱鍵,縮寫
        /// </summary>
        private Dictionary<string, string> _GetHotKeyAbbreviation = new Dictionary<string, string>();

        /// <summary>
        /// 縮寫,名稱
        /// </summary>
        private Dictionary<string, string> _GetAbbreviationName = new Dictionary<string, string>();

        /// <summary>
        /// 建立具有各種假別字典的物件
        /// </summary>
        public AbsenceHelper()
        {
            //DSResponse dsrsp_1 = Config.GetAbsenceList();
            //DSXmlHelper helper_1 = dsrsp_1.GetContent();
            foreach (AbsenceMappingInfo element in K12.Data.AbsenceMapping.SelectAll())
            {
                if (CheckAbsence(element)) //檢查
                {
                    _GetNameHotKey.Add(element.Name, element.HotKey);
                    _GetHotKeyName.Add(element.HotKey, element.Name);
                    _GetNameAbbreviation.Add(element.Name, element.Abbreviation);
                    _GetHotKeyAbbreviation.Add(element.HotKey, element.Abbreviation);
                    _GetAbbreviationName.Add(element.Abbreviation, element.Name);
                   
                }
                else
                {
                    MsgBox.Show("假別熱鍵有誤,請確認假別熱鍵");
                }
            }
        }

        /// <summary>
        /// 取得名稱熱鍵
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetListNameHotKey()
        {
            return _GetNameHotKey;
        }

        /// <summary>
        /// 取得名稱縮寫
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetListNameAbbreviation()
        {
            return _GetNameAbbreviation;
        }

        /// <summary>
        /// 取得熱鍵縮寫
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetListHotKeyAbbreviation()
        {
            return _GetHotKeyAbbreviation;
        }

        /// <summary>
        /// 以縮寫取得名稱
        /// </summary>
        /// <param name="Abbreviation"></param>
        /// <returns></returns>
        public string GetNameByAbbreviation(string Abbreviation)
        {
            if (_GetAbbreviationName.ContainsKey(Abbreviation))
            {
                return _GetAbbreviationName[Abbreviation];
            }
            else
            {
                return Abbreviation;
            }
        }

        /// <summary>
        /// 以名稱取得熱鍵
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public string GetHotKeyByName(string Name)
        {
            if(_GetNameHotKey.ContainsKey(Name))
            {
                return _GetNameHotKey[Name];
            }
            else
            {
                return Name;
            }
        }

        /// <summary>
        /// 以熱鍵取得名稱
        /// </summary>
        /// <param name="HotKey"></param>
        /// <returns></returns>
        public string GetNameByHotKey(string HotKey)
        {
            if (_GetHotKeyName.ContainsKey(HotKey))
            {
                return _GetHotKeyName[HotKey];
            }
            else
            {
                return HotKey;
            }
        }

        /// <summary>
        /// 以名稱取得縮寫
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public string GetAbbreviationByName(string Name)
        {
            if (_GetNameAbbreviation.ContainsKey(Name))
            {
                return _GetNameAbbreviation[Name];
            }
            else
            {
                return Name;
            }
        }

        /// <summary>
        /// 以熱鍵取得縮寫
        /// </summary>
        /// <param name="HotKey"></param>
        /// <returns></returns>
        public string GetAbbreviationByHotKey(string HotKey)
        {
            if (_GetHotKeyAbbreviation.ContainsKey(HotKey))
            {
                return _GetHotKeyAbbreviation[HotKey];
            }
            else
            {
                return HotKey;
            }
        }


        //public bool HotKeyExists(string key)
        //{
        //    foreach (KeyValuePair<string, string> each in _GetAbsence)
        //    {
                
        //    }

        //}

        /// <summary>
        /// 熱鍵是否存在
        /// </summary>
        /// <param name="HotKey"></param>
        /// <returns></returns>
        public bool HotKeyExists(string HotKey)
        {
            return _GetHotKeyName.ContainsKey(HotKey);
        }

        /// <summary>
        /// 縮寫是否存在
        /// </summary>
        /// <param name="Abbreviation"></param>
        /// <returns></returns>
        public bool AbbreviationExists(string Abbreviation)
        {
            return _GetHotKeyAbbreviation.ContainsValue(Abbreviation);
        }

        private bool CheckAbsence(AbsenceMappingInfo absc)
        {
            //檢查假別熱鍵表是否有值
            if (absc.Abbreviation == string.Empty || absc.HotKey == string.Empty || absc.Name == string.Empty)
            {
                return false;
            }
            return true;
        }
    }
}
