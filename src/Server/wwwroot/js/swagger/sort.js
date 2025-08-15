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
    let processedElements = new WeakSet();
    let observer = null;

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

        swaggerEndPointsContainers.forEach(x => {
            const opblock = x.querySelector(SwaggerEndPointClass);
            if (opblock && opblock.id) {
                swaggerEndPointIDs.push(opblock.id);
            }
        });

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

        swaggerEndPointIDsByHttpMethods.forEach(x => {
            const element = document.getElementById(x);
            if (element && element.parentNode) {
                swaggerEndPointsSorted.appendChild(element.parentNode);
            }
        });

        swaggerEndPointsSorted
            .classList
            .add(SwaggerEndPoints, SwaggerEndPointsSorted);

        if (swaggerEndPointsNotSorted.parentNode) {
            swaggerEndPointsNotSorted
                .parentNode
                .replaceChild(swaggerEndPointsSorted, swaggerEndPointsNotSorted);
        }
    }

    function SwaggerEndPointsToggleHandle(swaggerEndPointsToggle) {
        if (processedElements.has(swaggerEndPointsToggle)) {
            return;
        }
        processedElements.add(swaggerEndPointsToggle);

        let swaggerEndPointsNotSorted = swaggerEndPointsToggle.querySelectorAll(SwaggerEndPointsNotSortedClass);

        if (swaggerEndPointsNotSorted.length != 1)
            return;

        swaggerEndPointsNotSorted = swaggerEndPointsNotSorted[0];
        SwaggerEndPointsSort(swaggerEndPointsNotSorted);

        if (!swaggerEndPointsToggleClickHandlersAssigned) {
            swaggerEndPointsToggle.addEventListener("click", _ => {
                if (!swaggerEndPointsToggle.classList.contains(SwaggerEndPointsOpen)) {
                    const notSorted = swaggerEndPointsToggle.querySelector(SwaggerEndPointsNotSortedClass);
                    if (notSorted) {
                        SwaggerEndPointsSort(notSorted);
                    }
                }
            });

            swaggerEndPointsToggleClickHandlersAssigned = true;
        }
    }

    function processSwaggerEndPoints() {
        let swaggerEndPointsToggles = document.querySelectorAll(SwaggerEndPointsToggleClass);

        if (swaggerEndPointsToggles.length < 1)
            return;

        swaggerEndPointsToggles.forEach(SwaggerEndPointsToggleHandle);
    }

    function initObserver() {
        if (!document.body) {
            setTimeout(initObserver, 100);
            return;
        }

        if (observer) {
            observer.disconnect();
        }

        observer = new MutationObserver((mutations) => {
            let shouldProcess = false;

            for (const mutation of mutations) {
                if (mutation.type === 'childList' && mutation.addedNodes.length > 0) {
                    for (const node of mutation.addedNodes) {
                        if (node.nodeType === Node.ELEMENT_NODE) {
                            if (node.matches && (
                                node.matches(SwaggerEndPointsToggleClass) ||
                                node.querySelector(SwaggerEndPointsToggleClass)
                            )) {
                                shouldProcess = true;
                                break;
                            }
                        }
                    }
                }
                if (shouldProcess) break;
            }

            if (shouldProcess) {
                processSwaggerEndPoints();
            }
        });

        try {
            observer.observe(document.body, {
                childList: true,
                subtree: true
            });
            console.log('MutationObserver started successfully');
        } catch (error) {
            console.error('Error starting MutationObserver:', error);
            setInterval(processSwaggerEndPoints, 1000);
        }
    }

    function init() {
        processSwaggerEndPoints();
        initObserver();
    }

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', init);
    } else {
        init();
    }

    window.addEventListener('load', () => {
        setTimeout(processSwaggerEndPoints, 100);
    });

})();