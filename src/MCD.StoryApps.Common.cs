extern alias SHSharp;

using System;
using System.Text;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;


namespace MCD.StoryApps.Common
{
    public static class MCDCom
    {
		public static string GetAssetName(string name)
		{
			var assembly = Assembly.GetExecutingAssembly();
			string resourceName = assembly.GetManifestResourceNames().Single(str => str.EndsWith(name));

			return resourceName;
		}

		public static string AssetToText(string name)
		{
			var assembly = Assembly.GetExecutingAssembly();

			using (Stream stream = assembly.GetManifestResourceStream(name))
			using (StreamReader reader = new StreamReader(stream))
			{
				return reader.ReadToEnd();
			}
		}

		public static SHSharp::SHGUIsprite AddFrameFromStr(SHSharp::SHGUIsprite s, string content, int rowsPerFrame)
		{
			string[] array = content.Split('\n');
			int num = 0;
			string text = "";
			for (int i = 0; i < array.Length; i++)
			{
				text = text + array[i] + "\n";
				num++;
				if (num > rowsPerFrame - 1)
				{
					num = 0;
					s.AddFrame(text);
					text = "";
				}
			}

			return s;
		}

		public static SHSharp::SHGUIsprite AddSpecificFrameFromStr(SHSharp::SHGUIsprite s, string content, int rowsPerFrame, int addFrame, string sound = null)
		{
			string[] array = content.Split('\n');
			int num = 0;
			string text = "";
			int num2 = 0;
			for (int i = 0; i < array.Length; i++)
			{
				text = text + array[i] + "\n";
				num++;
				if (num > rowsPerFrame - 1)
				{
					num = 0;
					if (addFrame == num2)
					{
						s.AddFrame(text, sound);
					}

					text = "";
					num2++;
				}
			}

			return s;
		}

		public static Material LoadMaterialFromResource(string resourceName)
		{
			string fullResourceName = GetAssetName(resourceName);

			// Read the binary data from the embedded resource
			byte[] fileData;
			var assembly = Assembly.GetExecutingAssembly();

			using (Stream stream = assembly.GetManifestResourceStream(fullResourceName))
			{
				if (stream == null)
				{
					Debug.LogError($"Resource '{resourceName}' not found.");
					return null;
				}

				using (MemoryStream ms = new MemoryStream())
				{
					stream.CopyTo(ms);
					fileData = ms.ToArray();
				}
			}

			// Now we have the fileData as a byte array, which is a binary representation of the material properties
			Material mat = new Material(Shader.Find("Standard"));

			// Parse the fileData and set the material properties
			// This part of the code depends heavily on the structure of your data
			// Here's an example of setting some properties manually:

			mat.SetFloat("_Glossiness", 0.5f);
			mat.SetFloat("_Metallic", 0.8f);
			mat.SetColor("_Color", Color.red); // Assuming this color is within your data

			// You would add more parsing and setting of properties here based on the content of fileData

			return mat;
		}
	}
}
