Npgsql - .NET Data Provider for PostgreSQL
=============

###What Is Npgsql?

Npgsql is a .Net Data Provider for PostgreSQL. It allows you to connect and interact with PostgreSQL server using .NET.


###Project Information

+   [Offical Site][1]
+   [Npgsql source code @ GitHub][2]
+   [Available on NuGet Gallery][3]
+   [Wiki - Usage and other information][4]


###Developers

Developers who contributes to Npgsql currently or in past, in no particular order:
    
+   Brar Piening
+   Francisco Figueiredo Jr.
+   Ben Clewett
+   Daniel Morgan
+   Dave Page
+   Ulrich Sprick
+   Glen Parker
+   Josh Cooley
+   Jon Asher
+   Chris Morgan
+   Thilo Utke
+   Hiroshi Saito
+   Shay Rojansky

###License

Refer LICENSE.txt for more information on license terms.

###Code history

On December 2nd, [we committed a change][5] to restructure Npgsql code. This change modified file locations and unfortunately made the complete modification history of those files to not be available anymore through GitHub interface directly. [This stackoverflow post][6] documents a way to access the history before the rename: You just need to access the object tree **before** the rename and then you can see the rest of the file history.
In the case of this rename, this is the tree: https://github.com/npgsql/Npgsql/tree/2f8928e4dee59275767de0df2ec41a654744a3bc

This will help future searches for the history of the files. Note that you stil can access the complete history of the file through command line: `git log --follow ./newflodername/file`


[1]: http://www.npgsql.org/  "Official site"
[2]: https://github.com/npgsql/Npgsql/  "Npgsql source code @ GitHub"
[3]: http://www.nuget.org/packages/Npgsql/ "Npgsql @ Nuget Gallery"
[4]: https://github.com/npgsql/Npgsql/wiki/  "Wiki"
[5]: https://github.com/npgsql/Npgsql/commit/d7beea23f3407b38747cde05568a59ac87cdd121#diff-3b02da674650ba0f31603c365249f34f
[6]: http://stackoverflow.com/questions/17213046/see-history-in-github-after-folder-rename
