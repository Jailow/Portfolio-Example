using System.IO;
using UnityEngine;

public class RootChecker : MonoBehaviour
{
    public static bool isDeviceRooted()
    {
        bool rooted = false;
        string[] paths = new string[14] {
             "/Applications/Cydia.app",
             "/private/var/lib/cydia",
             "/private/var/tmp/cydia.log",
             "/System/Library/LaunchDaemons/com.saurik.Cydia.Startup.plist",
             "/usr/libexec/sftp-server",
             "/usr/bin/sshd",
             "/usr/sbin/sshd",
             "/Applications/FakeCarrier.app",
             "/Applications/SBSettings.app",
             "/Applications/WinterBoard.app",
             "/Library/MobileSubstrate/MobileSubstrate.dylib",
             "/etc/apt",
             "/bin/hash",
             "/private/var/lib/apt/",};

        for (int i = 0; i < paths.Length; i++)
            if (File.Exists(paths[i]))
                rooted = true;

        if (Application.sandboxType == ApplicationSandboxType.SandboxBroken)
            rooted = true;

        /*if (AntiDebug.IsDebuggerPresent())
            rooted = true;*/

        /*if (CertificateVerifer.CertificateIntegrityCheck())
            rooted = true;*/

        return rooted;
    }
}