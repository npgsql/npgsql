# NpgsqlRawReplicationStream.cs

This file had to be manually adjusted many times, most of them because of type and property name changes on dependencies, and Interface changes.

## Manual changes

### 1. `Read` method

This method make a call to the underlying `NpgsqlReader.ReadAllBytes` method, which was removed and replaced by a new implementation callled `ReadBytes`. I am not sure if the changes that I made were correct. Need to review this.