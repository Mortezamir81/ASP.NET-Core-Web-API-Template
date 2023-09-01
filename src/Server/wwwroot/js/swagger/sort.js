"use strict";

(function () {
	const SwaggerEndPointsOpen = "is-open";
	const SwaggerEndPoints = "operation-tag-content";
	const SwaggerEndPointsSorted = "swagger-end-points-sorted";

	const SwaggerEndPointClass = ".opblock";
	const SwaggerEndPointsToggleClass = ".opblock-tag-section";
	const SwaggerEndPointsNotSortedClass = `.${SwaggerEndPoints}:not(.${SwaggerEndPointsSorted})`;

	const SwaggerHttpMethods = Object.freeze([
		"head",
		"options",
		"get",
		"post",
		"patch",
		"put",
		"delete"
	]);

	let swaggerEndPointsToggleClickHandlersAssigned = false;

	function GetSwaggerEndPointIDHttpMethodIndex(swaggerEndPointID) {
		for (let i = 0; i < SwaggerHttpMethods.length; i++)
			if (swaggerEndPointID.includes(`-${SwaggerHttpMethods[i]}_`))
				return i;
	}

	function SortSwaggerEndPointIDs(swaggerEndPointIDX, swaggerEndPointIDY) {
		let swaggerEndPointIDHttpMethodIndeX = GetSwaggerEndPointIDHttpMethodIndex(swaggerEndPointIDX);
		let swaggerEndPointIDHttpMethodIndeY = GetSwaggerEndPointIDHttpMethodIndex(swaggerEndPointIDY);

		if (swaggerEndPointIDHttpMethodIndeX < swaggerEndPointIDHttpMethodIndeY)
			return -1;

		if (swaggerEndPointIDHttpMethodIndeX > swaggerEndPointIDHttpMethodIndeY)
			return 1;

		return 0;
	}

	function GetSwaggerEndPointIDsByHttpMethods(swaggerEndPointsContainers) {
		let swaggerEndPointIDs = [];

		swaggerEndPointsContainers.forEach(x => swaggerEndPointIDs.push(x.querySelector(SwaggerEndPointClass).id));
		swaggerEndPointIDs.sort();
		swaggerEndPointIDs.sort(SortSwaggerEndPointIDs);

		return swaggerEndPointIDs;
	}

	function SwaggerEndPointsSort(swaggerEndPointsNotSorted) {
		let swaggerEndPointsContainers = swaggerEndPointsNotSorted.querySelectorAll(":scope > span");

		if (swaggerEndPointsContainers.length < 1)
			return;

		let swaggerEndPointsSorted = document.createElement("div");
		let swaggerEndPointIDsByHttpMethods = GetSwaggerEndPointIDsByHttpMethods(swaggerEndPointsContainers);

		swaggerEndPointIDsByHttpMethods.forEach(x => swaggerEndPointsSorted.appendChild(document.getElementById(x).parentNode));

		swaggerEndPointsSorted
			.classList
			.add(SwaggerEndPoints, SwaggerEndPointsSorted);

		swaggerEndPointsNotSorted
			.parentNode
			.replaceChild(swaggerEndPointsSorted, swaggerEndPointsNotSorted);
	}

	function SwaggerEndPointsToggleHandle(swaggerEndPointsToggle) {
		let swaggerEndPointsNotSorted = swaggerEndPointsToggle.querySelectorAll(SwaggerEndPointsNotSortedClass);

		if (swaggerEndPointsNotSorted.length != 1)
			return;

		swaggerEndPointsNotSorted = swaggerEndPointsNotSorted[0];
		SwaggerEndPointsSort(swaggerEndPointsNotSorted);

		if (swaggerEndPointsToggleClickHandlersAssigned)
			return;

		swaggerEndPointsToggle.addEventListener("click", _ => {
			if (!swaggerEndPointsToggle.classList.contains(SwaggerEndPointsOpen))
				SwaggerEndPointsSort(swaggerEndPointsNotSorted);
		});

		swaggerEndPointsToggleClickHandlersAssigned = true;
	}

	window.addEventListener("DOMNodeInserted", () => {
		let swaggerEndPointsToggles = document.querySelectorAll(SwaggerEndPointsToggleClass);

		if (swaggerEndPointsToggles.length < 1)
			return;

		swaggerEndPointsToggles.forEach(SwaggerEndPointsToggleHandle);
	});
})();