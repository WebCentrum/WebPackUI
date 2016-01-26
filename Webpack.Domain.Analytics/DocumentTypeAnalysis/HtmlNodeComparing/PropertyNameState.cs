using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Webpack.Domain.Analytics.Extensions;

namespace Webpack.Domain.Analytics.DocumentTypeAnalysis.HtmlNodeComparing
{
    public class PropertyNameState
    {
        public const string MODEL_PREFIX = "@Model.";
        public const string PROPERTY_PREFIX = "Property";

        private int counterInitialy = 0;
        private int counter;

        //private string propertyName;
        private PropertyDTO currentProperty;

        public PropertyNameState()
        {
            counter = counterInitialy;
        }

        public bool Check(Queue<HtmlNode> referenceQueue)
        {
            if (referenceQueue == null || referenceQueue.IsEmpty())
            {
                throw new ArgumentNullException("referenceQueue");
            }

            if (IsPropertyNode(referenceQueue.Peek()))
            {
                var node = referenceQueue.Dequeue();
                var propertyName = node.InnerText.Substring(MODEL_PREFIX.Length);
                currentProperty = new PropertyDTO(propertyName, MODEL_PREFIX + propertyName);
                return true;
            }
            return false;
        }

        public void Reset()
        {
            currentProperty = null;
        }

        //public string GetTemplateProperty()
        //{
        //    return MODEL_PREFIX + GetName();
        //}

        public virtual PropertyDTO GetProperty()
        {
            if (currentProperty == null)
	        {
		        var name = PROPERTY_PREFIX + counter++.ToString();
                return new PropertyDTO(name, MODEL_PREFIX + name );
            }
            return currentProperty;
        }
        
        private bool IsPropertyNode(HtmlNode htmlNode)
        {
            return htmlNode.NodeType == HtmlNodeType.Text && htmlNode.InnerText.StartsWith(MODEL_PREFIX + PROPERTY_PREFIX);
        }
    }

    public class PropertyDTO
    {
        public PropertyDTO(string name, string templateReference)
        {
            Name = name;
            TemplateReference = templateReference;
        }

        public string Name { get; private set; }
        public string TemplateReference { get; private set; }
    }
}
