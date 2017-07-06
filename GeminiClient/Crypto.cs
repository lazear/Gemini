/*
MIT License

Copyright (c) Michael Lazear 2017 

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using System;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace Gemini
{
	class Cryptography
	{
		public static string SHA256Sign(string message, string key)
		{
			using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(message)))
			{
				var signed = new HMACSHA256(Encoding.UTF8.GetBytes(key)).ComputeHash(stream)
					.Aggregate(new StringBuilder(), (sb, b) => sb.AppendFormat("{0:x2}", b), (sb) => sb.ToString());
				return signed;
			}
		}

		public static string SHA384Sign(string message, string key)
		{
			using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(message)))
			{
				var signed = new HMACSHA384(Encoding.UTF8.GetBytes(key)).ComputeHash(stream)
					.Aggregate(new StringBuilder(), (sb, b) => sb.AppendFormat("{0:x2}", b), (sb) => sb.ToString());
				return signed;
			}
		}

		public static string SHA512Sign(string message, string key)
		{
			using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(message)))
			{
				var signed = new HMACSHA384(Encoding.UTF8.GetBytes(key)).ComputeHash(stream)
					.Aggregate(new StringBuilder(), (sb, b) => sb.AppendFormat("{0:x2}", b), (sb) => sb.ToString());
				return signed;
			}
		}

		public static byte[] GenerateSalt(int length)
		{
			byte[] salt = new byte[length];
			using (var rng = new RNGCryptoServiceProvider())
			{
				rng.GetBytes(salt);
			}
			return salt;
		}

		public static void AesEncryption(Stream input, string file, string password)
		{
			using (var encrypted = new FileStream(file, FileMode.Create, FileAccess.Write))
			{
				byte[] salt = GenerateSalt(32);
				var AES = new RijndaelManaged()
				{
					KeySize = 256,
					BlockSize = 128,
					Padding = PaddingMode.PKCS7,
				};

				var key = new Rfc2898DeriveBytes(password, salt, 1024);
				AES.Key = key.GetBytes(AES.KeySize / 8);
				AES.IV = key.GetBytes(AES.BlockSize / 8);

				AES.Mode = CipherMode.CFB;

				encrypted.Write(salt, 0, salt.Length);

				var cs = new CryptoStream(encrypted, AES.CreateEncryptor(), CryptoStreamMode.Write);
				var buffer = new byte[4096];
				int read;

				while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
					cs.Write(buffer, 0, read);
				cs.FlushFinalBlock();
			}
		}

		public static Stream AesDecryption(string file, string password)
		{

			Stream output = new MemoryStream();
			using (Stream input = new FileStream(file, FileMode.Open, FileAccess.Read))
			{
				byte[] salt = new byte[32];
				input.Read(salt, 0, 32);

				var AES = new RijndaelManaged()
				{
					KeySize = 256,
					BlockSize = 128,
					Padding = PaddingMode.PKCS7,
				};

				var key = new Rfc2898DeriveBytes(password, salt, 1024);
				AES.Key = key.GetBytes(AES.KeySize / 8);
				AES.IV = key.GetBytes(AES.BlockSize / 8);

				AES.Mode = CipherMode.CFB;

				var cs = new CryptoStream(input, AES.CreateDecryptor(), CryptoStreamMode.Read);
				var buffer = new byte[4096];
				int read;

				while ((read = cs.Read(buffer, 0, buffer.Length)) > 0)
					output.Write(buffer, 0, read);
			}
			output.Seek(0, SeekOrigin.Begin);
			return output;
		}
	}

}
