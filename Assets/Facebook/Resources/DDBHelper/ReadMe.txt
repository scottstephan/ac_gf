//----------------------------------------------
// DynamoDB Helper
// Copyright � 2014 OuijaPaw Games LLC
//----------------------------------------------

------------------------------
Changelog
------------------------------
v 1.0.0:
- Initial release of DDBHelper

v 1.1.0
- New:		Implemented 'querymultiple' method for DBWorker
- New:		Added query options for strongly consistent/eventually consistent reads
- New:		Added equality operators for queries
- New:		Created an 'all-in-one' async example
- New:		Centralized common methods into a DBTools.cs file
- Change:	Determined that monobehaviors absolutely cannot be high-level DynamoDB objects
- Change:	Moved DBContext out of DBWorker class so it's more accessible to other methods
- Change:	Moved main directory of DDBHelper out of Plugins
- Delete:	Unnecessary AWS DLLs
- Fix:		DBUnityHelper may have had a null reference if object was destroyed

v 1.2.0
- New:		Queries can use secondary indexes now
- New:      AWS SDK Source Code
- New/Fix:  AWS SDK Modified to run on iOS
- New:      Editor headings so it looks vaguely pretty 
- Fix:		Queries will only compare for equality on hash keys (as they should)
- Fix:		Test load/save now starts coroutine correctly and works
- Fix:		Certificate check callback update for iOS
- Change:	All delegate returns now have exceptions contained within
- Change:	Removed AWS XML file, reducing package size by 2 MB
- Change:   Removed unnecessary AWS SDK code, package size is VERY small now
- Change:   iOS version cannot run as DLL, included additional .unity package

v 1.3.0
- New:		Added access to DynamoDBLocal
- New:		Added Describe/Create/Delete/Update table functionality
- New:		Added simple scan method to view all objects in DB.  Recommended only for DynamoDB local,
			as scanning the remote DB will consume read/write and is very expensive.  This is basically
			a way to see what's in your DynamoDB local.
- New:		Added method for listing all tables (again, useful for DynamoDBLocal)
- New:		Basic DynamoDBLocal instructions added
- Fix:		Exists method was set up incorrectly, resolved

v 1.4.0
- New:		DBObject.GetHashKey() and DBObject.GetRangeKey() method
- New:		DBObject has option to be case-sensitive or not
- New:		DBObject provides debug info if data you request doesn't exist
- New:		Added a DateTime converter example to DBDataConverters
- Fix:		RegionEndpoint no longer defaults to USEast1 and uses the helper settings
- Fix:		Readitem callback was not triggering for null result
- Change:	Updated log messages for low level API to provide more information
- Change:	PrintException provides better information

v 1.5.0
- New:		Updated AWS SDK DLL to use version 2.3.17.0
- New:		Boolean Types Available:  DBObject.GetBool DBOBject.SetBool  DBObject.PrepareUpdateBool
- New:		Created DBAccountExample to show inline class loading/saving - much more efficient than other methods
- New:		Added DBWorker Load method for using hash/range strings instead of premade objects

v 2.0.0
- New:		Updated AWS SDK to Unity Mobile version for IL2CPP compatibility
- New:		AWS Cognito Identity optional for connections to protect AWS AccessID and SecretID
- New:		Optional platform information at startup
- New:      Optional Verbose logging for all data
- New:		DBObject now handles adding a List<int>, List<List<int>>, and List<List<List<int>>> for nested Lists
- New:		DBWorker has new QueryHashKeyOnly method for checking for only if hashkey exists
- New:      Scan is a proper, robust method now
- New:      ScanAdvanced for more advanced scanning.  Scanning consumed high read units, beware.
- New:      Where available, Debug 'on' results in HTTP response and content length
- New:      Scan with Debug 'on' shows consumed units
- Fix:      Describe table was working improperly at times
- Change:   Minimum verions for iOS due to IL2CPP is Unity patch 4.6.3P3 or later
- Change:	All non-async methods removed, replaced with strict async methods
- Change:	Due to async method updates, worker thread does not create worker queues anymore
- Change:	Updated Important Information and Install Instructions
- Remove:	The async example is removed, as everything is now using an async method
- Remove:   iOS package should no longer be needed for compatibility
- Notice:   The warning for:
				Assets/DDBHelper/AWSSDK/src/Core/Amazon.Runtime/Internal/Util/HashStream.cs(281,27): warning CS0436: 
				The type `System.IO.InvalidDataException' conflicts with the imported type `System.IO.InvalidDataException'. 
				Ignoring the imported type definition
			This is warning ONLY if you are using API Compatibility for full .NET 2.0.  This exception class is created for
			the subset .NET 2.0 version, which it is not included and why it is there.  It is a harmless warning.  I'll have
			to look at some preprocessor directive if possible to detect the player setting.  

v 2.1.2
- New:		DBHashKeyContextQuery example file for new QueryHashKeyObject method
- New:		QueryHashKeyObject<T> DBWorker method for getting a list of objects of type T using only hashkey
- New:		QueryObject<T> DBWorker method for getting a list of objects of Type T using hashkey and rangekey+operator
- Fix:		Some DBDataConverter examples were testing strings incorrectly.  Fixed.
- Change:	Verbose logging turned on by default.  Important information #6 has instructions on disabling it.
- Learn:	When connecting with a Cognito Identity Pool, be aware that unless you're connecting to an authentication
			provider (facebook, amazon, etc.) you are an UNAUTHORIZED connection.  If you do not have role policies to
			handle unauthenticated cognito connections for the DynamoDB table/database, it will fail.

v 2.2.0
- Update:	AWS SDK updated to current version
- New:		Support for Enum types without conversion now supported
- New:		Added Enum type for example to DBAccountExample.cs to show it in use
- New:		Added 'AWS Cognito Setup Primer' Document to demonstrate how to setup Cognito

v 2.3.0
- Update:	Unity 4.6+ supported now.  Previous 4.x version no longer supported.
- Fix:		Unity 5.3 uses a 'Logger' class which was causing conflicts
- Update:	AWS SDK Updated to v3

v 2.3.1
- Update:	AWS SDK Updated to v3.1
- Fix:		Change examples to work with pre Unity 5.3 versions


------------------------------
Important Information
------------------------------
1.  You must have a AWS account and access to a DynamoDB instance.

2.  If you plan on using AWS Congnito, verify it usable in your region, as it is not available everywhere:
	http://aws.amazon.com/about-aws/global-infrastructure/regional-product-services/

3.	If you are using AWS Cognito, here is some information on setting it up:
	http://docs.aws.amazon.com/mobile/sdkforunity/developerguide/cognito-identity.html

	The Pool ID you're looking for can be found via:
	Login to AWS Console in web browser
	Select IAM Service from available services
	Select Roles from left-side bar
		- If you have not created roles for your Cognito Identities, do so now via 'Create New Role'
			- Roles need Policies in order to do 'stuff'
			- Make a Role for Authorized and Unauthorized users
			- Use Managed policy such as AmazonDynamoDBFullAccess or AmazonDynamoDBReadOnlyAccess
			- Or create your own fine grained policy
			- Only 2 Managed policies may be attached to a role at one time
			- You may also create a Role Policy for more control
		- Now that you have your role... Scroll down to the bottom of the screen
		- Bottom right-hand corner should have 'conditions' as to when the role is trusted
		- The condition for 'stringEquals' will show your Cognito ID Pool
		- e.g. us-east-1:x80000df-0s00-0q09-q00q-qw009090877q
	Be aware that if you are not using an authentication provider (facebook, amazon, google, etc.) that any
	connections are UNAUTHORIZED connections.  If you do not have role policies to handle unauthenticated 
	cognito connections for the DynamoDB table/database, it will fail.

4.	Cognito is not required; however, be forewarned that placing your AWS AccessID and SecretID in the
	file is not very secure.  Extracting data from a built Unity application is trivial.  I originally built
	this as a 'server product' for a client/server model architecture, so the client never connected to the
	database ever.  As such, AWS Cognito should alleviate this drawback.  Also, you could create specific user IDs
	via IAM to access the database and give them control to only resources as you allow.  Just be aware of this
	before you hand out the master keys to the kingdom.

5.  WebPlayer will not work due to inability of Amazon server to return a crossdomain.xml file.  
    Amazon is considering implementing this; however, they use different technology for this functionality which Unity 
	does not support, so it more than likely will not be added.

6.  To enable/disable really, really Verbose logging, open:
	Assets/DDBHelper/AWSSDK/src/Core/Resources/awsconfig.xml

	<logging logTo="UnityLogger"
         logResponses="Always"
         logMetrics="true"
         logMetricsFormat="JSON" />

	This setting is used to configure logging in Unity. When you log to UnityLogger, the framework internally prints the output
	to the Debug Logs. If you want to log HTTP responses, set the logResponses flag - the values can be Always, Never, or OnError. 
	You can also log performance metrics for HTTP requests using the logMetrics property, the log format can be specified using 
	LogMetricsFormat property, valid values are JSON or standard.

OLD #### Invalid as of version 2.0.
    #### You MUST use .NET 2.0 when compiling a built, not subset of .NET 2.0.  Using a subset will create the build to fail.  
	#### It now uses the AWS Mobile SDK, meaning subset 2.0 Works ####
    #### File size greatly reduced!  Rejoice!

OLD #### 99% invalid as of version 2.0, at least for mobile.  Probably unecessary, but left here for legacy reasons
    #### Due to Monodevelop (the platform, not editor) using its own certificate store, and not the current computer's 
    #### certificate store, all certificates are rejected.  Since connecting to DynamoDB uses certificate based encryption, 
    #### certificates are needed.  The DDBHelper attempts to circumvent this; however, over VPN and other rare instances it 
    #### has shown to fail.  The workaround is to use mozroots.exe, provided in Unity's mono install, so install the 
    #### computer's certificates into mono's certificate store.

    #### To accomplish this:
    #### - search your Unity install folder (not project folder) for mozroots.exe, which is in something akin to 
    #### "C:\Program Files (x86)\Unity\Editor\Data\MonoBleedingEdge\lib\mono\4.0\"   Note this folder.  
    #### - Open a command prompt and navigate to this folder.
    #### - Use:  mozroots.exe --import --sync  (linux will require sudo)

    #### Information on this can be found at:
    #### http://mono-project.com/FAQ:_Security


------------------------------
Setup
------------------------------
1.  Create Amazon Web Services (AWS) account
2.  Create a DynamoDB table
    - For testing this package, create a table with these properties (Case Matters!):
	    - Name = DDBHelper
		- HashKey = Name (data type String)
		- RangeKey = Type (data type String)
    - Note that the free-tier DynamoDB limits are 25 read/second and 25 write/sec
	- Note that indexes can only be created when table is created
3.  Record access key and secret key somewhere
4.  Import DDBHelper package.  It includes the necessary AWS DynamoDB SDK files.
5.  Create an empty GameObject in the scene
6.  Add the DBUnityHelper script object to the object, fill out various information.
7.  Add the DBUnityTest script to the object.  Select the first checkbox.
8.  Run the scene.  Console should indicate connection and information regarding table.
	- try the DDB_Normal connection with your Access ID / Secret Key first
	- if using DDB_Cognito connection, did you set up Cognito for unauthorized users?  (See Cognito section below)
9.  Select second checkbox, unselect first.
10. Run scene again.  Console should show information being saved and then loaded.

Go about examining the code how this accomplished.


------------------------------
Cognito Tips and Setup
------------------------------
In-Depth Information:  https://docs.aws.amazon.com/mobile/sdkforunity/developerguide/cognito-identity.html
There is a document named 'AWS Cognito Setup Primer' to view, which will aid you in setting this up.


------------------------------
Troubleshooting
------------------------------
iOS
- If I'm using iOS, am I on Unity version 4.6.3P3 or later?

Cognito
- Did I create a cognito ID?
- Did I assign a role?
- Did I assign a policy to the role?
- Is Cognito offered in my data region?
- Did I use an actual, valid cognito pool ID?

DynamoDB Local
- Did I set the port?
- Is DynamoDB Local running?
- Can I access the file?

WebPlayer
- WebPlayer doesn't work

General
- Turn on debugging
- Turn on verbose logging
- Do I have enough read/write capacity?
- Do I have access to the DynamoDB table?
- Check for typos
- Establish basic connection via a 'list tables' or 'describe table'
- Is my firewall open allowing a connection?

------------------------------
Information
------------------------------
The basic idea is this:

Unity is single threaded.  As such, you have two choices: 
1a. Have a monobehavior that has AWS DDB data properties
    + all data is in one spot
	- issues when saving and object is destroyed
or 

1b. Have an object with AWS DDB properties that a Monobehavior references
	+ data is broken up and can be reused (saving repeated loads, caching, etc.)
	- more complex

2.  Create the object.  View example2 for a decent way to set this up.
    Add relevant information.  Adding new data later doesn't cause issues.
	DBDataTemplate has a stripped down version of how it works.

3.  From the owning object, as needed, save the data object.  Use the delegate
    method parameter to get a reply, or point it to the next spot in your code.
	Alternatively, you could use a coroutine to wait for a save or load to complete.
	

Theoretical Example at high level:
Create a gameobject
Add Player script
Add PlayerData script
Player script 
    - add reference to PlayerData script
PlayerData script 
    - create data types, data converters
    - create save, load, delete, saveSpecificAttributes, etc.
    - just copy from examples for easy reference
During Player Start()
    - create PlayerData object
	- Load data
Use delegate to return boolean saying it has completed, or to run next method.
Have coroutine waiting for boolean/method
During gameplay, call Save as needed
    -Save whole thing, or single attributes
Delete when player dies

Theoretical Example #2 at high level:
Create a gameobject
Add Player script
Player script 
	- create inner Class for PlayerData as a AWS Table Reference
	    - create data types, data converters
	    - create save, load, delete, saveSpecificAttributes, etc.
    - just copy from examples for easy reference
During Player Start()
    - create PlayerData object
	- Load data
Use delegate to return boolean saying it has completed, or to run next method.
Have coroutine waiting for boolean/method
During gameplay, call Save as needed
    -Save whole thing, or single attributes
Delete when player dies
