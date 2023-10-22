using CreateTxtFiles;

FileProcess.CreateFiles();
FileProcess.deleteAll();
await FileProcess.ProcessMultipleWritesAsync();
FileProcess.deleteAll();

//await CreateTxtFiles.ProcessMultipleWritesAsync();
//await CreateTxtFiles.CreateFiles(filesCount);