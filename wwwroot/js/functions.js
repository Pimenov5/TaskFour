import { alertbox } from "/js/alertbox.js";

export async function isValidResponse(response, status) {
	var title = "ERROR!";
	var message = "";

	if (response == null)
		message = "Server response is null";
	else if (response.status !== status) {
		title = "ERROR (" + response.status + ")";
		const json = await response.json();
		message = json == null ? "Server response JSON is null" : json;
	}
	else
		return true;

	alertbox.render({
		alertIcon: 'error',
		title: title,
		message: message,
		btnTitle: 'OK',
		themeColor: '#ff054c',
		border: true
	});

	return false;
}