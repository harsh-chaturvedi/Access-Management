
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Template;

namespace Access_Management.Client.Extensions
{
    public static class RouteMatcher
    {
        public static bool Match(string routeTemplate, string requestPath)
        {
            var template = TemplateParser.Parse(routeTemplate);

            var matcher = new TemplateMatcher(template, GetDefaults(template));

            var result = new RouteValueDictionary();
            var match = matcher.TryMatch(requestPath, result);

            return match;
        }

        // This method extracts the default argument values from the template.
        private static RouteValueDictionary GetDefaults(RouteTemplate parsedTemplate)
        {
            var result = new RouteValueDictionary();

            foreach (var parameter in parsedTemplate.Parameters)
            {
                if (parameter.DefaultValue != null)
                {
                    result.Add(parameter.Name, parameter.DefaultValue);
                }
            }

            return result;
        }
    }
}