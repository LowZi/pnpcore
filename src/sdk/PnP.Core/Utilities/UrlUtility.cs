﻿using System;
namespace PnP.Core.Utilities
{
    // Taken and slightly adapted from https://raw.githubusercontent.com/pnp/PnP-Sites-Core/master/Core/OfficeDevPnP.Core/Utilities/UrlUtility.cs
    /// <summary>
    /// Static methods to modify URL paths.
    /// </summary>
    public static class UrlUtility
    {
        const char PATH_DELIMITER = '/';

        /// <summary>
        /// Combines a path and a relative path.
        /// </summary>
        /// <param name="path">A SharePoint URL</param>
        /// <param name="relativePaths">SharePoint relative URLs</param>
        /// <returns>Returns combined path with a relative paths</returns>
        public static Uri Combine(Uri path, params string[] relativePaths)
        {
            return Combine(path?.ToString(), relativePaths);
        }

        /// <summary>
        /// Combines a path and a relative path.
        /// </summary>
        /// <param name="path">A SharePoint URL</param>
        /// <param name="relativePaths">SharePoint relative URLs</param>
        /// <returns>Returns combined path with a relative paths</returns>
        public static Uri Combine(string path, params string[] relativePaths)
        {
            string pathBuilder = path ?? string.Empty;

            if (relativePaths == null)
                return new Uri(pathBuilder);

            foreach (string relPath in relativePaths)
            {
                pathBuilder = CombineInternal(pathBuilder, relPath);
            }
            return new Uri(pathBuilder);
        }

        /// <summary>
        /// Combines a path and a relative path.
        /// </summary>
        /// <param name="path">A SharePoint URL</param>
        /// <param name="relative">SharePoint relative URL</param>
        /// <returns>Returns comibed path with a relative path</returns>
        public static Uri Combine(string path, string relative)
        {
            return new Uri(CombineInternal(path, relative));
        }

        private static string CombineInternal(string path, string relative)
        {
            if (relative == null)
                relative = string.Empty;

            if (path == null)
                path = string.Empty;

            if (relative.Length == 0 && path.Length == 0)
                return string.Empty;

            if (relative.Length == 0)
                return path;

            if (path.Length == 0)
                return relative;

            path = path.Replace('\\', PATH_DELIMITER);
            relative = relative.Replace('\\', PATH_DELIMITER);

            return path.TrimEnd(PATH_DELIMITER) + PATH_DELIMITER + relative.TrimStart(PATH_DELIMITER);
        }

        /// <summary>
        /// Returns absolute URL of a resource located in a SharePoint site.
        /// </summary>
        /// <param name="webUrl">The URL of a SharePoint site (Web).</param>
        /// <param name="serverRelativeUrl">Any server relative URL of a resource.</param>
        /// <returns></returns>
        public static Uri MakeAbsoluteUrl(Uri webUrl, string serverRelativeUrl)
        {
            if (null == webUrl) return null;

            string serverUrl = $"{webUrl.Scheme}://{webUrl.Authority}";
            return new Uri(CombineInternal(serverUrl, serverRelativeUrl));
        }

        /// <summary>
        /// Ensure the absolute URL from a specified resource URL
        /// </summary>
        /// <param name="webUrl">The URL of a SharePoint site (Web).</param>
        /// <param name="resourceUrl">The absolute or server relative URL of a resource.</param>
        /// <param name="checkIfWebContainedResource">Indicates if the resource URL must belong to the specified web (default = false)</param>
        /// <returns>The absolute URL of the specified resource.</returns>
        public static Uri EnsureAbsoluteUrl(Uri webUrl, string resourceUrl, bool checkIfWebContainedResource = false)
        {
            if (null == resourceUrl) throw new ArgumentNullException(nameof(resourceUrl));
            if (null == webUrl) throw new ArgumentNullException(nameof(webUrl));

            if (resourceUrl.StartsWith("https://"))
            {
                if (checkIfWebContainedResource && !resourceUrl.StartsWith(webUrl.ToString()))
                    throw new ArgumentException($"The {nameof(resourceUrl)} is not a resource from the SharePoint site {webUrl}");

                return new Uri(resourceUrl);
            }

            return MakeAbsoluteUrl(webUrl, resourceUrl);
        }

        /// <summary>
        /// Checks wether the resource absolute or relative URL is located in specified site (Web).
        /// </summary>
        /// <param name="webUrl">The URL of the SharePoint site (Web).</param>
        /// <param name="resourceUrl">The absolute or relative URL of a resource.</param>
        /// <returns><c>true</c> if the resource is in the same site, <c>false</c> otherwise</returns>
        public static bool IsSameSite(Uri webUrl, string resourceUrl)
        {
            if (null == webUrl) throw new ArgumentNullException(nameof(webUrl));
            if (string.IsNullOrEmpty(resourceUrl)) throw new ArgumentNullException(nameof(resourceUrl));

            string resourceAbsoluteUrl = EnsureAbsoluteUrl(webUrl, resourceUrl).ToString();
            return resourceAbsoluteUrl.ToLower().StartsWith(webUrl.ToString().ToLower());
        }

        #region NOT USED NOW
        // NOT Used for now, not covered by unit tests
        //const string INVALID_CHARS_REGEX = @"[\\#%*/:<>?+|\""]";
        //const string REGEX_INVALID_FILEFOLDER_NAME_CHARS = @"[""#%*:<>?/\|\t\r\n]";
        ///// <summary>
        ///// Returns relative URL of given URL
        ///// </summary>
        ///// <param name="urlToProcess">SharePoint URL to process</param>
        ///// <returns>Returns realtive URL of given URL</returns>
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1055:Uri return values should not be strings", Justification = "<Pending>")]
        //public static string MakeRelativeUrl(string urlToProcess)
        //{
        //    Uri uri = new Uri(urlToProcess);
        //    return uri.AbsolutePath;
        //}

        ///// <summary>
        ///// Adds query string parameters to the end of a querystring and guarantees the proper concatenation with <b>?</b> and <b>&amp;.</b>
        ///// </summary>
        ///// <param name="path">A SharePoint URL</param>
        ///// <param name="queryString">Query string value that need to append to the URL</param>
        ///// <returns>Returns URL along with appended query string</returns>
        //public static string AppendQueryString(string path, string queryString)
        //{
        //    string url = path;

        //    if (null != path && queryString != null && queryString.Length > 0)
        //    {
        //        char startChar = (path.IndexOf("?") > 0) ? '&' : '?';
        //        url = string.Concat(path, startChar, queryString.TrimStart('?'));
        //    }
        //    return url;
        //}


        ///// <summary>
        ///// Ensures that there is a trailing slash at the end of the URL
        ///// </summary>
        ///// <param name="urlToProcess"></param>
        ///// <returns></returns>
        //public static string EnsureTrailingSlash(string urlToProcess)
        //{
        //    if (null != urlToProcess && !urlToProcess.EndsWith("/"))
        //    {
        //        return urlToProcess + "/";
        //    }

        //    return urlToProcess;
        //}

        ///// <summary>
        ///// Checks if URL contains invalid characters or not
        ///// </summary>
        ///// <param name="content">Url value</param>
        ///// <returns>Returns true if URL contains invalid characters. Otherwise returns false.</returns>
        //public static bool ContainsInvalidUrlChars(this string content)
        //{
        //    return Regex.IsMatch(content, INVALID_CHARS_REGEX);
        //}

        ///// <summary>
        ///// Checks if file or folder contains invalid characters or not
        ///// </summary>
        ///// <param name="content">File or folder name to check</param>
        ///// <returns>True if contains invalid chars, false otherwise</returns>
        //public static bool ContainsInvalidFileFolderChars(this string content)
        //{
        //    return Regex.IsMatch(content, REGEX_INVALID_FILEFOLDER_NAME_CHARS);
        //}

        ///// <summary>
        ///// Removes invalid characters
        ///// </summary>
        ///// <param name="content">Url value</param>
        ///// <returns>Returns URL without invalid characters</returns>
        //public static Uri StripInvalidUrlChars(this string content)
        //{
        //    return ReplaceInvalidUrlChars(content, "");
        //}

        ///// <summary>
        ///// Replaces invalid charcters with other characters
        ///// </summary>
        ///// <param name="content">Url value</param>
        ///// <param name="replacer">string need to replace with invalid characters</param>
        ///// <returns>Returns replaced invalid charcters from URL</returns>
        //public static Uri ReplaceInvalidUrlChars(this string content, string replacer)
        //{
        //    return new Uri(new Regex(INVALID_CHARS_REGEX).Replace(content, replacer));
        //}
        #endregion
    }
}
