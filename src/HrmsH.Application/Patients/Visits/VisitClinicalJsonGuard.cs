using System.Text;
using System.Text.Json;
using HrmsH.Domain.Patients;

namespace HrmsH.Application.Patients.Visits;

/// <summary>
/// Validates and normalizes versioned clinical JSON. Hard limits protect the DB and API from abuse at scale.
/// </summary>
public static class VisitClinicalJsonGuard
{
    public const int MaxUtf8Bytes = 384 * 1024;

    /// <summary>
    /// Returns null for GENERAL when input is null/empty. Otherwise returns canonical JSON text or throws <see cref="InvalidOperationException"/>.
    /// </summary>
    public static string? NormalizeOrThrow(string? clinicalDataJson, string visitFormTemplate)
    {
        var trimmed = string.IsNullOrWhiteSpace(clinicalDataJson) ? null : clinicalDataJson.Trim();
        if (trimmed is null)
        {
            if (visitFormTemplate == VisitFormTemplates.General)
                return null;
            trimmed = """{"v":1}""";
        }

        var byteCount = Encoding.UTF8.GetByteCount(trimmed);
        if (byteCount > MaxUtf8Bytes)
            throw new InvalidOperationException($"Clinical data exceeds maximum size ({MaxUtf8Bytes} bytes).");

        JsonElement root;
        try
        {
            root = JsonSerializer.Deserialize<JsonElement>(trimmed);
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException("Clinical data is not valid JSON.", ex);
        }

        if (root.ValueKind != JsonValueKind.Object)
            throw new InvalidOperationException("Clinical data must be a JSON object.");

        if (!root.TryGetProperty("v", out var vProp) || vProp.ValueKind != JsonValueKind.Number)
            throw new InvalidOperationException("Clinical data must include a numeric version property \"v\".");

        if (!vProp.TryGetDouble(out var v) || v < 1)
            throw new InvalidOperationException("Clinical data version \"v\" must be >= 1.");

        return JsonSerializer.Serialize(root, new JsonSerializerOptions { WriteIndented = false });
    }
}
