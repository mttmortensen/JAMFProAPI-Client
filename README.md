# Jamf Pro API Toolkit (4Life Internal Tools)

This is a personal toolkit built in C# for interacting with the [Jamf Pro API](https://developer.jamf.com/) to assist with common device management tasks such as FileVault user retrieval, recovery key handling, and LAPS management.

> ⚠️ **Note:** This repo is a sanitized version of internal tooling used at my job. All sensitive data (URLs, tokens, device names, etc.) has been removed or replaced with placeholders. API tokens and credentials are injected via environment variables for safety.

---

## 🔧 Features

- ✅ Retrieve FileVault 2 user list by computer ID
- 🔑 Get and clear recovery lock keys
- 🔒 Query and manage Local Admin Password Service (LAPS) settings
- 🧠 Utility methods for converting computer names to IDs, and IDs to management IDs

---

## 🗂 Project Structure

| File              | Purpose                                                                 |
|-------------------|-------------------------------------------------------------------------|
| `Program.cs`       | Example usage of the FileVault2 module                                  |
| `TokenManager.cs`  | Handles OAuth token generation and caching                              |
| `ApiManager.cs`    | Base class providing common Jamf API helper methods                     |
| `FileVault2.cs`    | Module for retrieving FileVault 2 user lists                            |
| `RecoveryKeys.cs`  | Module to retrieve and clear recovery keys from computers               |
| `LAPS.cs`          | Module to retrieve LAPS settings and accounts                           |

---

## 🧪 Future Plans
- Add better CLI options for modular tool usage
- Add support for CSV output
- Mock API responses for safe, offline testing
- Add unit tests using fake/mock data

---

## 💡 Why This Exists
At my job, we manage hundreds of Macs via Jamf Pro. Many tasks like fetching FileVault users or clearing recovery locks require repetitive, manual steps via the web UI. This tool speeds up the workflow by wrapping those calls in easy-to-use C# code.  
This public version demonstrates my ability to:
- Work with OAuth-secured APIs
- Structure and abstract reusable API calls
- Parse and handle JSON and XML responses
- Build developer tooling to streamline IT workflows

---

## 🔒 Disclaimer
This repo does not contain real client data, Jamf credentials, or production device information. All data has been anonymized, and the project structure is safe for public viewing.

---

## 🛠 Environment Setup

To run this project, you must set three environment variables on your system. These are used for authentication and API access and are loaded securely through the abstract `ApiManager` class.

```bash
# Required Environment Variables

JAMF_URL=https://your-jamf-instance.jamfcloud.com
JAMF_CLIENT_ID=your-client-id-here
JAMF_CLIENT_SECRET=your-client-secret-here
```
---

### `.env.example` for reference

```env
JAMF_URL=https://your-jamf-instance.jamfcloud.com
JAMF_CLIENT_ID=your-client-id-here
JAMF_CLIENT_SECRET=your-client-secret-here
```

> ⚠️ This file should never be committed with real credentials. It's for reference only.


---

## 🧠 Author
Matt Mortensen  
System Admin & Software Developer  
🔗 github.com/mttmortensen  
