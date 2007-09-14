function MM_findObj(n, d) { //v3.0
  var p,i,x;  if(!d) d=document; if((p=n.indexOf("?"))>0&&parent.frames.length) {
    d=parent.frames[n.substring(p+1)].document; n=n.substring(0,p);}
  if(!(x=d[n])&&d.all) x=d.all[n]; for (i=0;!x&&i<d.forms.length;i++) x=d.forms[i][n];
  for(i=0;!x&&d.layers&&i<d.layers.length;i++) x=MM_findObj(n,d.layers[i].document); return x;
}

function MM_showHideLayers() { //v3.0
  var i,p,v,obj,args=MM_showHideLayers.arguments;
  for (i=0; i<(args.length-2); i+=3) if ((obj=MM_findObj(args[i]))!=null) { v=args[i+2];
    if (obj.style) { obj=obj.style; v=(v=='show')?'visible':(v='hide')?'hidden':v; }
    obj.visibility=v; }
  if (v=='visible') { document.search.searchstring.focus(); }
  if (v=='show') { document.search.searchstring.focus(); }
}

//Pop-up Window Script
function popUp(pPage) {
  window.open(pPage,'popWin','resizable=yes,scrollbars=yes,width=550,height=420,toolbar=no');
}

//Pop-up Window For Add to MyInformIT
function popAdd(pPage) {

	var newWin;

	newWin = window.open(pPage,'popAdd','resizable=no,scrollbars=no,width=550,height=148,toolbar=no');
}

function popDel(pPage) {

	var newWin;

	newWin = window.open(pPage,'popDel','resizable=no,scrollbars=no,width=550,height=148,toolbar=no');
	newWin.document.close();
	location.reload();
}