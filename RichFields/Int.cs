using System;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Routing;

namespace RichFields
{
    public static class Int
    {
        public const string DEFAULT_FMT = "###,###,###,##0;(###,###,###,##0);0";

        /// <summary>
        /// HTML Helper for Int RichField
        /// </summary>
        /// 
        /// <typeparam name="TModel">Type of the model</typeparam>
        /// <typeparam name="TValue">Type of the value</typeparam>
        /// <param name="helper">HTML Helper</param>
        /// <param name="expression">Value expression supplied</param>
        /// <param name="DisplayFormat">
        ///     Optional: if supplied overrides the default format.
        /// </param>
        /// <param name="htmlAttributes">
        ///     optional object specifying HTML attributes to append
        /// </param>
        /// <returns>The html to inject in the razor page.</returns>
        public static MvcHtmlString rfEditIntFor<TModel, TValue>(
            this HtmlHelper<TModel> helper,
            Expression<Func<TModel, TValue>> expression,
            string DisplayFormat = DEFAULT_FMT,
            object htmlAttributes = null)
        {
            var config = new FieldConfig<TModel, TValue>(helper, expression,
                htmlAttributes);

            config.InputType = "text";
            config.InputClass = "rf-int rounded-right";
            config.ScrubType = "number";
            config.InitialDisplayValue = string.Format("{0:" + DisplayFormat + "}",
                config.Value);

            var inputTag = config.InputTag();
            // display the proper keyboard on mobile browsers
            inputTag.Attributes.Add("inputmode", "numeric");
            inputTag.Attributes.Add("pattern", "[,0-9]*");

            config.OriginalTag();
            config.ScrubbedTag();

            return config.GetHtmlString();
        }
    }
}
