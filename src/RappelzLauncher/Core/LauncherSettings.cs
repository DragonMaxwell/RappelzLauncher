using System.Security.Cryptography;
using System.Text;

namespace RappelzLauncher.Core;

public sealed class LauncherSettings
{
    // @@Note_001 - I know it's insecure, but you can implement your own method to retrieve the encryption key.
    private const string EncryptionKey = "ABCDF0123456789";

    private const string DefaultIniAccount = "account";
    private const string DefaultIniPassword = "password";
    private const bool DefaultIniUseAutoLogin = false;
    private const string DefaultIniParameters = "RappelzCmdLauncher.exe SFrame.exe /auth_ip:127.0.0.1 /auth_port:4615 /help_url_w:611 /help_url_h:625 /locale:ASCII /country:CN /cash /gift /commercial_shop /layout_dir:6 /layout_auto:0 /cash_url_w:800 /cash_url_h:631 /use_nprotect:0";
    private const string DefaultIniLauncherUrl = "https://your_server_address";


    public string Account { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool UseAutoLogin { get; set; }
    public string Parameters { get; set; } = string.Empty;
    public string LauncherUrl { get; set; } = string.Empty;
    
    private static readonly string DefaultIniTemplate = string.Join("\r\n", new[]
    {
        "#########################",
        "[Login]",
        $"account={DefaultIniAccount}",
        $"password={DefaultIniPassword}",
        $"useAutoLogin={DefaultIniUseAutoLogin}",
        "[SframeParams]",
        $"param={DefaultIniParameters}",
        $"launcherUrl={DefaultIniLauncherUrl}",
        "#########################",
        ""
    });

    public void Save(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        EnsureIniEncrypted();
        
        byte[] key = SHA256.HashData(Encoding.UTF8.GetBytes(EncryptionKey));
        byte[] originalBytes;

        if (File.Exists(path))
        {
            byte[] storedBytes = File.ReadAllBytes(path);

            if (storedBytes.Length < 29)
                throw new InvalidDataException("Invalid encrypted configuration file.");

            byte[] nonce = storedBytes.AsSpan(0, 12).ToArray();
            byte[] tag = storedBytes.AsSpan(12, 16).ToArray();
            byte[] ciphertext = storedBytes.AsSpan(28).ToArray();

            originalBytes = new byte[ciphertext.Length];

            using AesGcm aes = new(key, 16);
            aes.Decrypt(nonce, ciphertext, tag, originalBytes);
        }
        else
        {
            Account = DefaultIniAccount;
            Password = DefaultIniPassword;
            UseAutoLogin = DefaultIniUseAutoLogin;
            Parameters = DefaultIniParameters;
            LauncherUrl = DefaultIniLauncherUrl;

            originalBytes = Encoding.UTF8.GetBytes(DefaultIniTemplate);
        }

        bool hasBom = originalBytes.AsSpan().StartsWith(Encoding.UTF8.GetPreamble());
        int offset = hasBom ? Encoding.UTF8.GetPreamble().Length : 0;

        string original = Encoding.UTF8.GetString(originalBytes, offset, originalBytes.Length - offset);
        string newline = original.Contains("\r\n", StringComparison.Ordinal) ? "\r\n" : "\n";
        string[] lines = original.Replace("\r\n", "\n", StringComparison.Ordinal).Split('\n');
        string section = string.Empty;

        for (int index = 0; index < lines.Length; index++)
        {
            string trimmed = lines[index].Trim();

            if (trimmed.Length == 0 || trimmed.StartsWith('#') || trimmed.StartsWith(';'))
                continue;

            if (trimmed.StartsWith('[') && trimmed.EndsWith(']'))
            {
                section = trimmed[1..^1].Trim();
                continue;
            }

            int separator = lines[index].IndexOf('=');

            if (separator < 1)
                continue;

            string name = lines[index][..separator].Trim();

            string? updatedValue = section.ToLowerInvariant() switch
            {
                "login" when name.Equals("account", StringComparison.OrdinalIgnoreCase) => Clean(Account),
                "login" when name.Equals("password", StringComparison.OrdinalIgnoreCase) => Clean(Password),
                "login" when name.Equals("useAutoLogin", StringComparison.OrdinalIgnoreCase) => UseAutoLogin ? "true" : "false",
                _ => null
            };

            if (updatedValue is null)
                continue;

            string currentValue = lines[index][(separator + 1)..];

            if (currentValue.Trim() == updatedValue)
                continue;

            int leadingSpaces = currentValue.Length - currentValue.TrimStart().Length;
            int trailingSpaces = currentValue.Length - currentValue.TrimEnd().Length;

            string leading = currentValue[..leadingSpaces];
            string trailing = trailingSpaces == 0 ? string.Empty : currentValue[^trailingSpaces..];

            lines[index] = lines[index][..(separator + 1)] + leading + updatedValue + trailing;
        }

        string updatedContent = string.Join(newline, lines);
        byte[] plaintext = Encoding.UTF8.GetBytes(updatedContent);

        if (hasBom)
            plaintext = Encoding.UTF8.GetPreamble().Concat(plaintext).ToArray();

        byte[] newNonce = RandomNumberGenerator.GetBytes(12);
        byte[] newTag = new byte[16];
        byte[] ciphertextResult = new byte[plaintext.Length];

        using (AesGcm aes = new(key, 16))
        {
            aes.Encrypt(newNonce, plaintext, ciphertextResult, newTag);
        }

        byte[] result = new byte[28 + ciphertextResult.Length];

        Buffer.BlockCopy(newNonce, 0, result, 0, 12);
        Buffer.BlockCopy(newTag, 0, result, 12, 16);
        Buffer.BlockCopy(ciphertextResult, 0, result, 28, ciphertextResult.Length);

        using FileStream stream = new(path, File.Exists(path) ? FileMode.Open : FileMode.CreateNew, FileAccess.Write, FileShare.None);

        stream.Position = 0;
        stream.Write(result);
        stream.SetLength(result.Length);
        stream.Flush(true);
    }

    public static LauncherSettings Load(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        EnsureIniEncrypted();
        
        byte[] key = SHA256.HashData(Encoding.UTF8.GetBytes(EncryptionKey));

        if (!File.Exists(path))
        {
            new LauncherSettings().Save(path);
        }

        byte[] storedBytes = File.ReadAllBytes(path);

        if (storedBytes.Length < 29)
            throw new InvalidDataException("Invalid encrypted configuration file.");

        byte[] nonce = storedBytes.AsSpan(0, 12).ToArray();
        byte[] tag = storedBytes.AsSpan(12, 16).ToArray();
        byte[] ciphertext = storedBytes.AsSpan(28).ToArray();

        byte[] plaintext = new byte[ciphertext.Length];

        using (AesGcm aes = new(key, 16))
        {
            aes.Decrypt(nonce, ciphertext, tag, plaintext);
        }

        int offset = plaintext.AsSpan().StartsWith(Encoding.UTF8.GetPreamble()) ? Encoding.UTF8.GetPreamble().Length : 0;
        string content = Encoding.UTF8.GetString(plaintext, offset, plaintext.Length - offset);

        Dictionary<string, string> values = new(StringComparer.OrdinalIgnoreCase);
        string section = string.Empty;

        foreach (string line in content.Split('\n'))
        {
            string trimmed = line.Trim();

            if (trimmed.Length == 0 || trimmed.StartsWith('#') || trimmed.StartsWith(';'))
                continue;

            if (trimmed.StartsWith('[') && trimmed.EndsWith(']'))
            {
                section = trimmed[1..^1].Trim();
                continue;
            }

            int separator = trimmed.IndexOf('=');

            if (separator < 1)
                continue;

            string name = trimmed[..separator].Trim();
            string value = trimmed[(separator + 1)..].Trim();

            values[$"{section}.{name}"] = value;
        }

        string Read(string name) => values.TryGetValue(name, out string? value) ? value : string.Empty;

        return new LauncherSettings
        {
            Account = Read("Login.account"),
            Password = Read("Login.password"),
            UseAutoLogin = bool.TryParse(Read("Login.useAutoLogin"), out bool enabled) && enabled,
            Parameters = Read("SframeParams.param"),
            LauncherUrl = Read("SframeParams.launcherUrl")
        };
    }

    public Uri GetServerUri()
    {
        if (!Uri.TryCreate(LauncherUrl, UriKind.Absolute, out Uri? uri) || (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
        {
            throw new InvalidDataException("launcherUrl must contain a valid HTTP or HTTPS address in RappelzLauncher.ini.");
        }

        return uri;
    }

    private static string Clean(string value)
    {
        return value.Replace("\r", string.Empty, StringComparison.Ordinal).Replace("\n", string.Empty, StringComparison.Ordinal);
    }


    public static void EnsureIniEncrypted()
    {
        string filePath = Path.Combine(AppContext.BaseDirectory, "RappelzLauncher.ini");

        if (!File.Exists(filePath))
        {
            return;
        }

        byte[] storedBytes = File.ReadAllBytes(filePath);
        byte[] key = SHA256.HashData(Encoding.UTF8.GetBytes(EncryptionKey));

        if (storedBytes.Length >= 29)
        {
            byte[] nonce = storedBytes.AsSpan(0, 12).ToArray();
            byte[] tag = storedBytes.AsSpan(12, 16).ToArray();
            byte[] ciphertext = storedBytes.AsSpan(28).ToArray();
            byte[] plaintext = new byte[ciphertext.Length];

            try
            {
                using AesGcm aes = new(key, 16);
                aes.Decrypt(nonce, ciphertext, tag, plaintext);
                return;
            }
            catch (AuthenticationTagMismatchException)
            {
            }
        }

        string content;

        try
        {
            content = new UTF8Encoding(false, true).GetString(storedBytes);
        }
        catch (DecoderFallbackException)
        {
            throw new InvalidDataException("Invalid configuration file.");
        }

        if (!content.Contains("[Login]", StringComparison.OrdinalIgnoreCase) ||
            !content.Contains("[SframeParams]", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidDataException("Invalid or unsupported configuration file.");
        }

        byte[] nonceResult = RandomNumberGenerator.GetBytes(12);
        byte[] tagResult = new byte[16];
        byte[] ciphertextResult = new byte[storedBytes.Length];

        using (AesGcm aes = new(key, 16))
        {
            aes.Encrypt(nonceResult, storedBytes, ciphertextResult, tagResult);
        }

        byte[] result = new byte[28 + ciphertextResult.Length];

        Buffer.BlockCopy(nonceResult, 0, result, 0, 12);
        Buffer.BlockCopy(tagResult, 0, result, 12, 16);
        Buffer.BlockCopy(ciphertextResult, 0, result, 28, ciphertextResult.Length);

        File.WriteAllBytes(filePath, result);
    }


    public static void DecryptIniFile()
    {
        string filePath = Path.Combine(AppContext.BaseDirectory, "RappelzLauncher.ini");

        byte[] encrypted = File.ReadAllBytes(filePath);

        if (encrypted.Length < 29)
            throw new InvalidDataException("Invalid encrypted INI file.");

        byte[] key = SHA256.HashData(Encoding.UTF8.GetBytes(EncryptionKey));

        byte[] nonce = encrypted.AsSpan(0, 12).ToArray();
        byte[] tag = encrypted.AsSpan(12, 16).ToArray();
        byte[] ciphertext = encrypted.AsSpan(28).ToArray();
        byte[] plaintext = new byte[ciphertext.Length];

        using (AesGcm aes = new AesGcm(key, 16))
        {
            aes.Decrypt(nonce, ciphertext, tag, plaintext);
        }

        File.WriteAllBytes(filePath, plaintext);
    }
}
