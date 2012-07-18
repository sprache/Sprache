// ReSharper disable InconsistentNaming

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using NUnit.Framework;

namespace Sprache.Tests.Scenarios
{
    [TestFixture]
    public class AmqpErrorTests
    {
        static readonly Parser<char> itemSeparator = Parse.Char(',');
        static readonly Parser<char> stringDelimiter = Parse.Char('"');
        static readonly Parser<char> keyValueDelimiter = Parse.Char('=');

        static readonly Parser<char> stringContent = Parse.AnyChar.Except(stringDelimiter);

        static readonly Parser<StringValue> stringValue =
            from beingQuote in stringDelimiter
            from content in stringContent.XMany().Text()
            from endQuote in stringDelimiter
            select new StringValue(content);

        static readonly Parser<IntValue> intValue =
            from x in Parse.Number.Token()
            select new IntValue(int.Parse(x));

        static readonly Parser<Value> value =
            stringValue.Or<Value>(intValue);

        static readonly Parser<string> key =
            Parse.AnyChar.Except(keyValueDelimiter).Except(itemSeparator).XMany().Text();

        private static readonly Parser<KeyValue> keyValue =
            from k in key
            from x in keyValueDelimiter
            from v in value
            select new KeyValue(k, v);

        static readonly Parser<AmqpStringItem> itemContent =
            from x in Parse.AnyChar.Except(itemSeparator).XMany().Text()
            select new AmqpStringItem(x);

        static readonly Parser<AmqpErrorItem> item =
            keyValue.Or<AmqpErrorItem>(itemContent);

        private static readonly Parser<IEnumerable<AmqpErrorItem>> items =
            from leading in item
            from rest in itemSeparator.Then(_ => Parse.WhiteSpace).Then(_ => item).Many()
            select Cons(leading, rest);

        static IEnumerable<T> Cons<T>(T head, IEnumerable<T> rest)
        {
            yield return head;
            foreach (var item in rest)
                yield return item;
        }

    
        [Test]
        public void Should_parse_an_AMQP_error_string()
        {
            const string originalErrorString =
                "The AMQP operation was interrupted: AMQP close-reason, initiated by Peer, " +
                "code=406, text=\"PRECONDITION_FAILED - parameters for queue 'my.redeclare.queue' in vhost '/' not equivalent\", " +
                "classId=50, methodId=10, cause=";

            var itemsResult = 
                items.Parse(originalErrorString).OfType<KeyValue>().ToDictionary(x => x.Key, x => x.Value);
            
            foreach (var amqpErrorItem in itemsResult)
            {
                Console.Out.WriteLine("{0}", amqpErrorItem);
            }

            Console.Out.WriteLine("itemsResult[\"code\"] = {0}", itemsResult["code"]);
            Console.Out.WriteLine("itemsResult[\"text\"] = {0}", itemsResult["text"]);
            Console.Out.WriteLine("itemsResult[\"classId\"] = {0}", itemsResult["classId"]);
            Console.Out.WriteLine("itemsResult[\"methodId\"] = {0}", itemsResult["methodId"]);
        }
    }

    public class AmqpErrorItem
    {
    }

    public class AmqpStringItem : AmqpErrorItem
    {
        public string Text { get; private set; }

        public AmqpStringItem(string text)
        {
            Text = text;
        }

        public override string ToString()
        {
            return Text;
        }
    }

    public class KeyValue : AmqpErrorItem
    {
        public Value Value { get; private set; }
        public string Key { get; private set; }

        public KeyValue(string key, Value value)
        {
            Value = value;
            Key = key;
        }

        public override string ToString()
        {
            return string.Format("Key: '{0}', Value: '{1}'", Key, Value);
        }
    }

    public class Value
    {
        
    }

    public class StringValue : Value
    {
        public string Text { get; private set; }

        public StringValue(string text)
        {
            Text = text;
        }

        public override string ToString()
        {
            return Text;
        }
    }

    public class IntValue : Value
    {
        public int Value { get; private set; }

        public IntValue(int value)
        {
            Value = value;
        }

        public override string ToString()
        {
            return Value.ToString(CultureInfo.InvariantCulture);
        }
    }
}

// ReSharper restore InconsistentNaming