# Sm4shAIEditor

Plan: Open and edit attack_data, param, and script files for Smash 4's AI

**Current status**:

General: the GUI needs much more work to be fully functional.

Can open ai directories and import/export ATKD and script files.

Editing scripts can be done either through the GUI or through a decompiled output folder, but I haven't added a way to load fighters from that folder into the GUI yet.

Currently the GUI doesn't have a way to add or remove acts, but this can be done in the folder by removing the act ID from acts.txt.

Debug statements are in a primitive state, so I recommand using Visual Studio's debugger to find where an issue occurred.

**Building**

Open in Visual Studio and build. I'll make a release when it's more developed.
