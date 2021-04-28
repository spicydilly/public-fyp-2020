mergeInto(LibraryManager.library, {
  SendTheMapJson: function(utf8String) {
    var jsString = UTF8ToString(utf8String);
    SendTheJson(jsString);
  },
  LoadMaps: function() {
    LoadAllMaps();
  }, 
  LoadThisMap: function(id) {
	LoadAMap(id);  	
  }
});
