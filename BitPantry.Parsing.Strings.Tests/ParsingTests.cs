using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BitPantry.Parsing.Strings;
using BitPantry.Parsing.Strings.Configuration;

namespace BitPantry.Parsing.Strings.Tests
{
    [TestClass]
    public class ParsingTests
    {
        
        [TestMethod]
        public void Parse_string_returnsString()
        {
            var result = StringParsing.Parse<string>("hello");
        }

        [TestMethod]
        public void parse_1AsBool_returnsTrue()
        {
            var result = StringParsing.Parse<bool>("1");
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void parse_trueAsBool_returnsTrue()
        {
            var result = StringParsing.Parse<bool>("true");
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Parse_yesAsBool_returnsTrue()
        {
            var result = StringParsing.Parse<bool>("yes");
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Parse_0AsBool_returnsFalse()
        {
            var result = StringParsing.Parse<bool>("0");
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Parse_falseAsBool_returnsFalse()
        {
            var result = StringParsing.Parse<bool>("false");
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Parse_noAsBool_returnsFalse()
        {
            var result = StringParsing.Parse<bool>("no");
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Parse_asdfAsBool_returnsFalse()
        {
            var result = StringParsing.Parse<bool>("asdf");
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Parse_dateTimeMonthFirst_returnsDateTime()
        {
            var result = StringParsing.Parse<DateTime>("10/12/1979");
            Assert.AreEqual(result, new DateTime(1979,10,12));
        }

        [TestMethod]
        public void Parse_dateTimeYearFirst_returnsDateTime()
        {
            var result = StringParsing.Parse<DateTime>("2008/12/22");
            Assert.AreEqual(result, new DateTime(2008, 12, 22));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Parse_badDateString_throwsArgumentException()
        {
            var result = StringParsing.Parse<DateTime>("123/123/1232");
        }

        [TestMethod]
        public void Parse_stringList_returnsStringList()
        {
            var result = StringParsing.Parse<List<string>>("hello,goodbye");
            Assert.AreEqual(result.Count, 2);
        }

        [TestMethod]
        public void Parse_char_returnsChar()
        {
            var result = StringParsing.Parse<char>("a");
            Assert.AreEqual(result, 'a');
        }

        [TestMethod]
        public void Parse_charFromString_returnsCharAtIndexZero()
        {
            var result = StringParsing.Parse<char>("abc");
            Assert.AreEqual(result, 'a');
        }

        [TestMethod]
        public void Parse_charListWithStringValue_returnsCharListWithFirstCharOfString()
        {
            var result = StringParsing.Parse<List<char>>("a,bcd,e");
            Assert.AreEqual(result.Count, 3);
            Assert.AreEqual(result[0], 'a');
            Assert.AreEqual(result[1], 'b');
            Assert.AreEqual(result[2], 'e');
        }

        [TestMethod]
        public void Parse_charArray_returnsCharArray()
        {
            var result = StringParsing.Parse<char[]>("a,b,c");
            Assert.IsTrue(result.GetType() == typeof(char[]));
            Assert.IsTrue(result.Length == 3);
            Assert.IsTrue(result[0] == 'a');
            Assert.IsTrue(result[1] == 'b');
            Assert.IsTrue(result[2] == 'c');
        }

        [TestMethod]
        public void Parse_intList_returnsIntList()
        {
            var result = StringParsing.Parse<List<int>>("1,2,3,4");
            Assert.IsTrue(result.GetType() == typeof(List<int>));
            Assert.IsTrue(result.Count == 4);
            Assert.IsTrue(result[0] == 1);
            Assert.IsTrue(result[1] == 2);
            Assert.IsTrue(result[2] == 3);
            Assert.IsTrue(result[3] == 4);
        }

        [TestMethod]
        public void Parse_intListWithRange_returnsIntList()
        {
            var result = StringParsing.Parse<List<int>>("1,2-5,6,7");
            Assert.IsTrue(result.GetType() == typeof(List<int>));
            Assert.IsTrue(result.Count == 7);
            Assert.IsTrue(result[0] == 1);
            Assert.IsTrue(result[1] == 2);
            Assert.IsTrue(result[2] == 3);
            Assert.IsTrue(result[3] == 4);
            Assert.IsTrue(result[4] == 5);
            Assert.IsTrue(result[5] == 6);
            Assert.IsTrue(result[6] == 7);
        }

        [TestMethod]
        public void Parse_intArray_returnsIntArray()
        {
            var result = StringParsing.Parse<int[]>("1,2,3,4");
            Assert.IsTrue(result.GetType() == typeof(int[]));
            Assert.IsTrue(result.Length == 4);
            Assert.IsTrue(result[0] == 1);
            Assert.IsTrue(result[1] == 2);
            Assert.IsTrue(result[2] == 3);
            Assert.IsTrue(result[3] == 4);
        }

        [TestMethod]
        public void Parse_intArrayWithRange_returnsIntArray()
        {
            var result = StringParsing.Parse<int[]>("1,2-5,6,7");
            Assert.IsTrue(result.GetType() == typeof(int[]));
            Assert.IsTrue(result.Length == 7);
            Assert.IsTrue(result[0] == 1);
            Assert.IsTrue(result[1] == 2);
            Assert.IsTrue(result[2] == 3);
            Assert.IsTrue(result[3] == 4);
            Assert.IsTrue(result[4] == 5);
            Assert.IsTrue(result[5] == 6);
            Assert.IsTrue(result[6] == 7);
        }

        [TestMethod]
        public void SafeParse_badIntNoDefault_returnsDefaultValueForInt()
        {
            var i = StringParsing.SafeParse<int>("a");
            Assert.IsTrue(i == 0);
        }

        public void SafeParse_intNoDefault_returnsInt()
        {
            var i = StringParsing.SafeParse<int>("10");
            Assert.IsTrue(i == 10);
        }

        [TestMethod]
        public void SafeParse_badIntWithDefault_returnsDefault()
        {
            var i = StringParsing.SafeParse<int>("a", 100);
            Assert.IsTrue(i == 100);
        }

        [TestMethod]
        public void SafeParse_intWithDefault_returnsInt()
        {
            var i = StringParsing.SafeParse<int>("10", 20);
            Assert.IsTrue(i == 10);
        }
        
        [TestMethod]
        public void SafeParse_badIntListNoDefault_returnsEmptyIntListAsDefault()
        {
            var list = StringParsing.SafeParse<List<int>>("1,(,3,4");
            Assert.IsNotNull(list);
            Assert.AreEqual(list.GetType(), typeof(List<int>));
            Assert.AreEqual(list.Count, 0);
        }

        [TestMethod]
        public void SafeParse_badIntListWithDefault_returnsDefault()
        {
            var result = StringParsing.SafeParse<List<int>>("1, 2 - 5, #, 7", new List<int>() {10,20,30,40,50});
            Assert.IsTrue(result.GetType() == typeof(List<int>));
            Assert.IsTrue(result.Count == 5);
            Assert.IsTrue(result[0] == 10);
            Assert.IsTrue(result[1] == 20);
            Assert.IsTrue(result[2] == 30);
            Assert.IsTrue(result[3] == 40);
            Assert.IsTrue(result[4] == 50);
        }

        [TestMethod]
        public void SafeParse_intListNoDefault_returnsIntList()
        {
            var result = StringParsing.SafeParse<List<int>>("1,2,3,4");
            Assert.IsTrue(result.GetType() == typeof(List<int>));
            Assert.IsTrue(result.Count == 4);
            Assert.IsTrue(result[0] == 1);
            Assert.IsTrue(result[1] == 2);
            Assert.IsTrue(result[2] == 3);
            Assert.IsTrue(result[3] == 4);
        }

        [TestMethod]
        public void SafeParse_intListWithDefault_returnsIntList()
        {
            var result = StringParsing.SafeParse<List<int>>("1, 2 - 5, 6, 7", new List<int>() { 10,11,12 });
            Assert.IsTrue(result.GetType() == typeof(List<int>));
            Assert.IsTrue(result.Count == 7);
            Assert.IsTrue(result[0] == 1);
            Assert.IsTrue(result[1] == 2);
            Assert.IsTrue(result[2] == 3);
            Assert.IsTrue(result[3] == 4);
            Assert.IsTrue(result[4] == 5);
            Assert.IsTrue(result[5] == 6);
            Assert.IsTrue(result[6] == 7);
        }

        public enum StringParsingTestEnum
        {
            NoValue = 0,
            ThisIsValue1 = 1,
            ThisIsValue2 = 2,
        }

        [TestMethod]
        public void Parse_validStringAsEnum_returnsEnumValue()
        {
            var val = StringParsing.Parse<StringParsingTestEnum>("ThisIsValue1");
            Assert.AreEqual(val, StringParsingTestEnum.ThisIsValue1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Parse_invalidStringAsEnum_throwsArgumentOutOfRangeException()
        {
            StringParsing.Parse<StringParsingTestEnum>("ThisValueDoesNotExist");
        }

        [TestMethod]
        public void Parse_intAsEnum_returnsEnumValue()
        {
            var val = StringParsing.Parse<StringParsingTestEnum>("2");
            Assert.AreEqual(val, StringParsingTestEnum.ThisIsValue2);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Parse_invalidIntAsEnum_throwsArgumentOutOfRangeException()
        {
            StringParsing.Parse<StringParsingTestEnum>("3");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Parse_enumAsEnum_throwsArgumentOutOfRangeException()
        {
            StringParsing.Parse<StringParsingTestEnum>("3");
        }

        [TestMethod]
        public void Parse_enumNameList_returnsList()
        {
            var val = StringParsing.Parse<List<StringParsingTestEnum>>("ThisIsValue1, ThisIsValue2");
            Assert.AreEqual(val.GetType(), typeof(List<StringParsingTestEnum>));
            Assert.AreEqual(val.Count, 2);
            Assert.AreEqual(val[0], StringParsingTestEnum.ThisIsValue1);
            Assert.AreEqual(val[1], StringParsingTestEnum.ThisIsValue2);
        }

        [TestMethod]
        public void Parse_enumNameArray_returnArray()
        {
            var val = StringParsing.Parse<StringParsingTestEnum[]>("ThisIsValue1, ThisIsValue2");
            Assert.AreEqual(val.GetType(), typeof(StringParsingTestEnum[]));
            Assert.AreEqual(val.Length, 2);
            Assert.AreEqual(val[0], StringParsingTestEnum.ThisIsValue1);
            Assert.AreEqual(val[1], StringParsingTestEnum.ThisIsValue2);
        }

        [TestMethod]
        public void Parse_enumIntArray_returnArray()
        {
            var val = StringParsing.Parse<StringParsingTestEnum[]>("1, 2");
            Assert.AreEqual(val.GetType(), typeof(StringParsingTestEnum[]));
            Assert.AreEqual(val.Length, 2);
            Assert.AreEqual(val[0], StringParsingTestEnum.ThisIsValue1);
            Assert.AreEqual(val[1], StringParsingTestEnum.ThisIsValue2);
        }

        [TestMethod]
        public void Parse_enumIntList_returnsList()
        {
            var val = StringParsing.Parse<List<StringParsingTestEnum>>("0, 2");
            Assert.AreEqual(val.GetType(), typeof(List<StringParsingTestEnum>));
            Assert.AreEqual(val.Count, 2);
            Assert.AreEqual(val[0], StringParsingTestEnum.NoValue);
            Assert.AreEqual(val[1], StringParsingTestEnum.ThisIsValue2);
        }

        [TestMethod]
        public void Parse_keyValuePairStringString_keyValuePairStringString()
        {
            var val = StringParsing.Parse<KeyValuePair<string, string>>("apple=jelly");
            Assert.AreEqual(val.Key, "apple");
            Assert.AreEqual(val.Value, "jelly");
        }

        [TestMethod]
        public void Parse_keyValuePairStringBool_keyValuePairStringBool()
        {
            var val = StringParsing.Parse<KeyValuePair<string, bool>>("apple=true");
            Assert.AreEqual(val.Key, "apple");
            Assert.AreEqual(val.Value, true);
        }

        [TestMethod]
        public void Parse_keyValuePairIntEnum_keyValuePairIntEnum()
        {
            var val = StringParsing.Parse<KeyValuePair<int, StringParsingTestEnum>>("2=ThisIsValue1");
            Assert.AreEqual(val.Key, 2);
            Assert.AreEqual(val.Value, StringParsingTestEnum.ThisIsValue1);
        }

        [TestMethod]
        public void Parse_keyValuePairIntEnumEnum_keyValuePairIntEnumEnum()
        {
            var val = StringParsing.Parse<KeyValuePair<StringParsingTestEnum, StringParsingTestEnum>>("2=ThisIsValue1");
            Assert.AreEqual(val.Key, StringParsingTestEnum.ThisIsValue2);
            Assert.AreEqual(val.Value, StringParsingTestEnum.ThisIsValue1);         
        }

        [TestMethod]
        public void Parse_dictionaryDoubleEnum_dictionaryDoubleEnum()
        {
            var val = StringParsing.Parse<Dictionary<string, StringParsingTestEnum>>("noVal=0;one=ThisIsValue1;tttwo=2");
            Assert.AreEqual(val.Count, 3);
            Assert.AreEqual(val["noVal"], StringParsingTestEnum.NoValue);
            Assert.AreEqual(val["one"], StringParsingTestEnum.ThisIsValue1);
            Assert.AreEqual(val["tttwo"], StringParsingTestEnum.ThisIsValue2);
        }

        [TestMethod]
        public void Parse_crazyDictionary_returnsCrazyDictionary()
        {
            var val = StringParsing.Parse<Dictionary<string, List<int>>>("list One=1,2,3-5;listTwo=1,2,3");
            Assert.IsTrue(val.Count == 2);
            Assert.IsTrue(val["list One"].Count == 5);
            Assert.IsTrue(val["listTwo"].Count == 3);
        }

    }
}
