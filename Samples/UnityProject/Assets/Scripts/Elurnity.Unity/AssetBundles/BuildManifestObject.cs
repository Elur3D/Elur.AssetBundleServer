using UnityEngine;
using System.Collections.Generic;

#if !UNITY_CLOUD_BUILD
namespace UnityEngine.CloudBuild
{
    public interface BuildManifestObject
    {
        // Tries to get a manifest value - returns true if key was found and could be cast to type T, false otherwise.
        bool TryGetValue<T>(string key, out T result);

        // Retrieve a manifest value or throw an exception if the given key isn't found.
        T GetValue<T>(string key);

        // Sets the value for a given key.
        void SetValue(string key, object value);

        // Copy values from a dictionary. ToString() will be called on dictionary values before being stored.
        void SetValues(Dictionary<string, object> sourceDict);

        // Remove all key/value pairs
        void ClearValues();

        // Returns a Dictionary that represents the current BuildManifestObject
        Dictionary<string, object> ToDictionary();

        // Returns a JSON formatted string that represents the current BuildManifestObject
        string ToJson();

        // Returns an INI formatted string that represents the current BuildManifestObject
        string ToString();
    }
}
#endif