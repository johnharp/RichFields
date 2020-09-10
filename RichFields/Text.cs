using System;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Routing;

namespace RichFields
{
    public static class Text
    {
        /// <summary>
        /// HTML Helper for Text RichField.
        /// </summary>
        /// 
        /// <typeparam name="TModel">Type of the model</typeparam>
        /// <typeparam name="TValue">Type of the value</typeparam>
        /// <param name="helper">HTML Helper</param>
        /// <param name="expression">Value expression supplied</param>
        /// <param name="htmlAttributes">
        ///     optional object specifying HTML attributes to append
        /// </param>
        /// <returns>The html to inject in the razor page.</returns>
        public static MvcHtmlString rfEditTextFor<TModel, TValue>(
            this HtmlHelper<TModel> helper,
            Expression<Func<TModel, TValue>> expression,
            object htmlAttributes = null)
        {
            var config = new FieldConfig<TModel, TValue>(helper, expression,
                htmlAttributes);

            config.InputType = "text";
            config.InputClass = "rf-text rounded-right";
            config.InitialDisplayValue = config.InitialScrubbedValue;

            var inputTag = config.InputTag();

            config.OriginalTag();
            config.ScrubbedTag();

            return config.GetHtmlString();
        }
    }
}
