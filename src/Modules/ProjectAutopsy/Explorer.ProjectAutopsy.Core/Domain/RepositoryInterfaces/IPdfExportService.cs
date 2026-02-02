using Explorer.ProjectAutopsy.Core.Domain;

namespace Explorer.ProjectAutopsy.Core.Domain.RepositoryInterfaces;

/// <summary>
/// Service interface for exporting reports to PDF format.
/// </summary>
public interface IPdfExportService
{
    /// <summary>
    /// Generates a PDF report for a risk analysis snapshot.
    /// </summary>
    /// <param name="projectName">Name of the project being analyzed</param>
    /// <param name="snapshot">Risk analysis snapshot data</param>
    /// <param name="aiReport">Optional AI-generated report with insights</param>
    /// <returns>PDF file as byte array</returns>
    byte[] GenerateRiskAnalysisPdf(string projectName, RiskSnapshot snapshot, AIReport? aiReport = null);
}
