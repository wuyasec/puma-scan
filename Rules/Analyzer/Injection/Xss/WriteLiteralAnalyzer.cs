/* 
 * Copyright(c) 2016 - 2017 Puma Security, LLC (https://www.pumascan.com)
 * 
 * Project Leader: Eric Johnson (eric.johnson@pumascan.com)
 * Lead Developer: Eric Mead (eric.mead@pumascan.com)
 * 
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. 
 */

using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;

using Puma.Security.Rules.Common;
using Puma.Security.Rules.Diagnostics;
using Puma.Security.Rules.Model;
using Puma.Security.Rules.Regex;
using Puma.Security.Rules.Regex.Mvc;

using Microsoft.CodeAnalysis;

namespace Puma.Security.Rules.Analyzer.Injection.Xss
{
    [SupportedDiagnostic(DiagnosticId.SEC0103)]
    public class WriteLiteralAnalyzer : IAdditionalTextAnalyzer
    {
        private readonly IRegexHelper _writeLiteralRegexHelper;

        public WriteLiteralAnalyzer(WriteLiteralRegexHelper writeLiteralRegexHelper)
        {
            _writeLiteralRegexHelper = writeLiteralRegexHelper;
        }

        public IEnumerable<DiagnosticInfo> GetDiagnosticInfo(IEnumerable<AdditionalText> srcFiles,
            CancellationToken cancellationToken)
        {
            var result = new List<DiagnosticInfo>();

            foreach (var file in srcFiles)
            {
                var document = file.GetText(cancellationToken);

                var source = document.ToString();

                if (!_writeLiteralRegexHelper.HasMatch(source)) continue;

                foreach (Match match in _writeLiteralRegexHelper.GetMatches(source))
                {
                    result.Add(new DiagnosticInfo(file.Path, document.Lines.GetLinePosition(match.Index).Line,
                        source.Substring(match.Index, match.Length)));
                }
            }

            return result;
        }
    }
}