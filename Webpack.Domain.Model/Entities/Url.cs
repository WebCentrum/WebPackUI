// <copyright file="Url.cs" company="ÚVT MU">
//     Copyright (c) ÚVT MU. All rights reserved.
// </copyright>
// <author>Matej Chudo</author>
namespace Webpack.Domain.Model
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web;

    [DebuggerDisplay("url = {url}")]
    public class Url
    {
        private readonly string url;
        private readonly Uri uri;
        private readonly string[] segments;

        public Url(Uri uri)
        {
            if (uri == null)
            {
                throw new ArgumentNullException("url");
            }
            this.uri = uri;
            this.url = uri.PathAndQuery;
            this.segments = uri.Segments.Select(s => s.Trim().TrimEnd('/')).Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();
        }

        public string Protocol
        {
            get { return uri.Scheme; }
        }

        public string Host 
        {
            get { return uri.Host; } 
        }

        public string Path
        {
            get { return uri.AbsolutePath; }
        }

        public string Query
        {
            get { return uri.Query; }
        }

        public string Fragment
        {
            get { return uri.Fragment; }
        }

        public string[] Segments
        {
            get { return segments; }
        }

        public string LastSegment
        {
            get { return Segments.LastOrDefault(); }
        }

        public Uri Uri
        {
            get { return uri; }
        }

        public bool SegmentPredicate<TSegment>(int number, Func<string, TSegment> converter, Func<TSegment, bool> predicate)
        {
            if (converter == null)
            {
                throw new ArgumentNullException("converter");
            }

            return SegmentExists(number) && predicate(converter(Segments[number]));
        }

        public bool SegmentPredicate(int number, Func<string, bool> predicate)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }

            return SegmentExists(number) && predicate(Segments[number]);
        }

        public bool SegmentExists(int number)
        {
            if (number < 0)
            {
                throw new ArgumentOutOfRangeException("number");
            }
            
            return number < Segments.Length;
        }

        public bool QueryMultiParameterPredicate<TParameter>(string parameter, Func<string, TParameter> converter, Func<IEnumerable<TParameter>, bool> predicate)
        {
            if (string.IsNullOrWhiteSpace(parameter))
            {
                throw new ArgumentOutOfRangeException("parameter");
            }
            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }
            if (converter == null)
            {
                throw new ArgumentNullException("converter");
            }

            var collection = HttpUtility.ParseQueryString(Query);
            var csv = collection[parameter];
            if (csv == null)
            {
                return false;
            }

            var values = csv.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            return predicate(values.Select(v => converter(v)));
        }

        public bool QueryMultiParameterPredicate(string parameter, Func<IEnumerable<string>, bool> predicate)
        {
            return QueryMultiParameterPredicate(parameter, s => s, predicate);
        }

        public bool QueryParameterPredicate<TParameter>(string parameter, Func<string, TParameter> convertor, Func<TParameter, bool> predicate)
        {
            if (string.IsNullOrWhiteSpace(parameter))
            {
                throw new ArgumentOutOfRangeException("parameter");
            }
            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }
            if (convertor == null)
            {
                throw new ArgumentNullException("converter");
            }

            var collection = HttpUtility.ParseQueryString(Query);
            var value = collection[parameter];
            if (value == null || value.Contains(','))
            {
                return false;
            }

            return predicate(convertor(value));
        }

        public bool QueryParameterPredicate<TParameter>(string parameter, Func<string, bool> predicate)
        {
            return QueryParameterPredicate(parameter, predicate, s => s);
        }

        public bool QueryParameterPredicate<TParameter>(string parameter)
        {
            return QueryParameterPredicate(parameter, s => true, s => s);
        }

        public override string ToString()
        {
            return uri.ToString();
        }
    }
}
