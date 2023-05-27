# EversoulTBLDecryptor

## Description

EversoulTBLDecryptor is a decryption tool designed to decode the encrypted FlatBuffers data (`.tbl`) data file format that used in [Eversoul](https://eversoul.playkakaogames.com/).

Eversoul initially used a 'broken' JSON format for their table data, but with version 1456, they changes to an encrypted format utilizing the [FlatBuffers](https://github.com/google/flatbuffers). The encryption algorithm used by Eversoul is Rijndael, and the Key and IV are derived from a combination of the table version and a key magic value. The key magic value is embedded within the game binary, while the table version is dependent on the version of the table data obtained either from the game folder or `0` if obtained directly from the game's CDN server.

It's important to note that this program solely focuses on decrypting the raw binary FlatBuffers and does not do data dumping. To dump the data, you will need the data schema, which can be obtained by dumping the game's IL2CPP.

# Usage

- Locate the key magic value and replace it [here](https://github.com/Yuerino/EversoulTBLDecryptor/blob/214bbfee8a998119116f3f168fba4debb097e0a1/Program.cs#LL9C44-L9C44)
- Adjust the table data version accordingly [here](https://github.com/Yuerino/EversoulTBLDecryptor/blob/214bbfee8a998119116f3f168fba4debb097e0a1/Program.cs#L8)
- Compile the program
- Run it and enter the path to the table data folder
- The decrypted files will be generated and stored in the decrypt folder within the table data folder.
