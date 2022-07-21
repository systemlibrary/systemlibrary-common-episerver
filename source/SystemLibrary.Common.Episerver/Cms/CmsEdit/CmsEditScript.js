function __systemLibraryCommonEpiserver_warn(msg) {
    console.log('%c SystemLibraryCommonEpiserver: ' + msg, 'background: #a8e0af; color: #212121');
}

function __systemLibraryCommonEpiserver_updateProjectBarColor() {
    var applicationContainer = document.getElementById('applicationContainer');

    if (!applicationContainer) {
        __systemLibraryCommonEpiserver_warn("applicationContainer not yet loaded");
        return;
    }

    var projectBars = applicationContainer.getElementsByClassName('epi-project-mode-toolbar dijitLayoutContainer dijitContainer');

    if (!projectBars) {
        __systemLibraryCommonEpiserver_warn("projectBars not yet loaded");
        return;
    }
    var projectBar = projectBars[0];
    if (!projectBar || !projectBar.classList) {
        __systemLibraryCommonEpiserver_warn("projectBar not yet loaded");
        return;
    }

    var projectButtons = projectBar.getElementsByClassName('dijitReset dijitInline dijitButtonText');
    if (!projectButtons) {
        __systemLibraryCommonEpiserver_warn("projectButtons not yet loaded");
        return;
    }

    var projectButton = projectButtons[0];
    if (!projectButton || !projectButton.innerText) {
        __systemLibraryCommonEpiserver_warn("projectButtons not yet loaded");
        return;
    }

    var projectName = projectButton.innerText.toLowerCase();
    if (projectName === 'none (use primary drafts)' ||
        projectName === 'ingen' ||
        (projectName.includes('none') && projectName.includes('primary drafts'))) {
        if (projectBar.classList.contains('custom-common-episerver-active-project-bar-background-color')) {
            projectBar.classList.remove('custom-common-episerver-active-project-bar-background-color');
        }
    } else {
        if (!projectBar.classList.contains('custom-common-episerver-active-project-bar-background-color')) {
            projectBar.classList.add('custom-common-episerver-active-project-bar-background-color');
        }
    }
}


document.addEventListener("DOMContentLoaded", () => setTimeout(__systemLibraryCommonEpiserver_updateProjectBarColor, 1500));