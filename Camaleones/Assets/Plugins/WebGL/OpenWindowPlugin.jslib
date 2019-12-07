﻿var OpenWindowPlugin = {
    openWindow: function(link)
    {
    	var url = Pointer_stringify(link);
        document.onmouseup = function()
        {
        	window.open(url, "_blank");
        	document.onmouseup = null;
        }
    }
};

mergeInto(LibraryManager.library, OpenWindowPlugin);