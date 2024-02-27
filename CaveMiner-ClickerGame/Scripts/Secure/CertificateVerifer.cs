using System;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class CertificateVerifer : MonoBehaviour
{
    private static X509Certificate2 certificate;

    public static bool CertificateIntegrityCheck()
    {
        bool result = false;

        if (certificate == null)
            result = true;
        else
        {
            if (certificate.NotAfter < DateTime.Now)
                result = true;

            if (!certificate.Verify())
                result = true;
        }

        return result;
    }
}