# CSharpVitamins.ShortGuid
A convenience wrapper for dealing with URL-safe base64 encoded globally unique identifier (GUID), making a shorter string value (22 vs 36 characters long).

URL-safe base64? That's just a base64 string with well known special characters replaced (/, +) or removed (==) to make it a consistent 22 characters long _and_ URI friendly.

Originally seen on [@madskristensen](https://github.com/madskristensen)'s blog, this helper class that extended on his idea existed only as [a blog post from around 2007](https://www.singular.co.nz/2007/12/shortguid-a-shorter-and-url-friendly-guid-in-c-sharp/).


Available on [NuGet](https://www.nuget.org/packages/csharpvitamins.shortguid/). To install, run the following command in the Package Manager Console:

    PM> Install-Package CSharpVitamins.ShortGuid

### Strict Parsing (v2.0.0)
As of version 2.0.0, `ShortGuid` performs a sanity check when decoding strings to ensure they haven't been
tampered with, i.e. allowing the end of a Base64 string to be tweaked where it still produces that same
byte array to create the underlying Guid. Effectively there is "unused space" in the Base64 string which is
ignored, but will now result in an `FormatException` being thrown.

`ShortGuid` will never produce an invalid string, however if one is supplied, it could result in an unintended
collision where multiple URL-safe Base64 strings can point to the same Guid. To avoid this uncertainty, a
round-trip check is performed to ensure a 1-1 match with the input string.

Stick with version 1.1.0 if you require the old behaviour with opt-in strict parsing.

---

## The Gist

It takes a standard guid like this:

`c9a646d3-9c61-4cb7-bfcd-ee2522c8f633`

and shortens it to a smaller string like this:

`00amyWGct0y_ze4lIsj2Mw`

Ultimately the encoding/decoding can be distilled down a pair of one-liner functions. What the `ShortGuid` gives you is a new Type that both

1. Aids in parsing and converting between types (strings and Guids)
2. Denotes your intention of expecting this type of data in your code


---


## Using the ShortGuid

The ShortGuid is compatible with normal Guids and other ShortGuid strings. Let's see an example:

```csharp
Guid guid = Guid.NewGuid();
ShortGuid sguid1 = guid; // implicitly cast the guid as a shortguid
Console.WriteLine(sguid1);
Console.WriteLine(sguid1.Guid);
```

This produces a new guid, uses that guid to create a ShortGuid, and displays the two equivalent values in the console. Results would be something along the lines of:

`FEx1sZbSD0ugmgMAF_RGHw`  
`b1754c14-d296-4b0f-a09a-030017f4461f`

Or you can implicitly cast a string to a ShortGuid as well.

```csharp
string code = "Xy0MVKupFES9NpmZ9TiHcw";
ShortGuid sguid2 = code; // implicitly cast the string as a shortguid
Console.WriteLine(sguid2);
Console.WriteLine(sguid2.Guid);
```

Which produces the following:

`Xy0MVKupFES9NpmZ9TiHcw`  
`540c2d5f-a9ab-4414-bd36-9999f5388773`



### Flexible with your other data types

The ShortGuid is made to be easily used with the different types, so you can simplify your code. Take note of the following examples:

```csharp
// for a new ShortGuid, just like Guid.NewGuid()
ShortGuid sguid = ShortGuid.NewGuid();

// to cast the string "myString" as a ShortGuid,
string myString = "Xy0MVKupFES9NpmZ9TiHcw";

// the following 3 lines are equivalent
ShortGuid sguid = new ShortGuid(myString); // traditional
ShortGuid sguid = (ShortGuid)myString;     // explicit cast
ShortGuid sguid = myString;                // implicit cast

// Likewise, to cast the Guid "myGuid" as a ShortGuid
Guid myGuid = new Guid("540c2d5f-a9ab-4414-bd36-9999f5388773");

// the following 3 lines are equivalents
ShortGuid sguid = new ShortGuid(myGuid); // traditional
ShortGuid sguid = (ShortGuid)myGuid;     // explicit cast
ShortGuid sguid = myGuid;                // implicit cast
```

After you've created your ShortGuid's the 3 members of most interest are the original Guid value, the new short string (the short encoded guid string), and the ToString() method, which also returns the short encoded guid string.

```csharp
sguid.Guid;       // gets the Guid part
sguid.Value;      // gets the encoded Guid as a string
sguid.ToString(); // same as sguid.Value
```



### Easy comparison with guids and strings

You can also do equals comparison against the three types, Guid, string and ShortGuid like in the following example:

```csharp
Guid myGuid = new Guid("540c2d5f-a9ab-4414-bd36-9999f5388773");
ShortGuid sguid = (ShortGuid)"Xy0MVKupFES9NpmZ9TiHcw";

if (sguid == myGuid) {
  // logic if guid and sguid are equal
}

if (sguid == "Xy0MVKupFES9NpmZ9TiHcw") {
  // logic if string and sguid are equal
}
```


---


## Use within SQL

`ShortGuid.sql` is also included in the project source files if you want to work with ShortGuids directly in SQL Server (SQL 2005+).

```SQL
SET @sguid = dbo.EncodeShortGuid(@myUniqueIdentifier)
-- output 'Xy0MVKupFES9NpmZ9TiHcw'

SET @guid = dbo.DecodeShortGuid(@myShortGuidValue)
-- output 540c2d5f-a9ab-4414-bd36-9999f5388773
```
