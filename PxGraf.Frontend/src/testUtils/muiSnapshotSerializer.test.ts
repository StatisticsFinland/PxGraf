describe('muiSnapshotSerializer', () => {
    it('removes Emotion hashes while preserving meaningful classes', () => {
        const fragment = document.createDocumentFragment();
        const wrapper = document.createElement('div');
        wrapper.className = 'app-wrapper css-abc123-MuiBox-root';
        const button = document.createElement('button');
        button.className = 'MuiButtonBase-root MuiButton-root Mui-disabled MuiButton-root css-1nkm392-MuiButtonBase-root-MuiButton-root';
        button.textContent = 'Save';
        const label = document.createElement('span');
        label.className = 'css-0';
        label.textContent = 'Label';
        wrapper.append(button, label);
        fragment.appendChild(wrapper);

        expect(fragment).toMatchSnapshot();
    });
});