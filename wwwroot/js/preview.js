$("#foto").change(function () {
	var fileName = this.files[0].name;
	var fileSize = this.files[0].size;
	var mjerror = '';

	if (fileSize > 3000000) {
		mjerror = 'El archivo no debe superar los 3MB';
	} else {
		// recuperamos la extensi�n del archivo
		var ext = fileName.split('.').pop();

		// Convertimos en min�scula
		ext = ext.toLowerCase();

		switch (ext) {
			case 'jpg':
			case 'jpeg':
			case 'png':
				break;
			default:
				mjerror = 'El archivo no tiene la extensi�n adecuada';
		}
	}

	if (mjerror == '') {
		readURL(this);
	}
	else {
		alert(mjerror);
		this.value = ''; // reset del valor
		this.files[0].name = '';
	}

});

function readURL(input) {
	if (input.files && input.files[0]) {
		var reader = new FileReader();

		reader.onload = function (e) {
			$("#imagen").attr("src", e.target.result);
		}

		reader.readAsDataURL(input.files[0]); // leer el archivo string base64
	}
}