import React from 'react';
import { render, screen } from '@testing-library/react';
import { MultiselectableSelector } from './MultiselectableSelector';
import { IVariable, VariableType } from "types/cubeMeta";
import { IVisualizationSettings } from '../../../types/visualizationSettings';

const mockVariables: IVariable[] = [
	{
		code: 'cVar',
		name: { fi: 'cVarName' },
		note: { fi: 'cVarNote' },
		type: VariableType.Content,
		values: [
			{
				code: 'cVal',
				name: { fi: 'cValName' },
				note: { fi: 'cValNote' },
				isSum: false
			}
		]
	},
	{
		code: 'tVar',
		name: { fi: 'tVarName' },
		note: { fi: 'tVarNote' },
		type: VariableType.Time,
		values: [
			{
				code: 'tVal',
				name: { fi: 'tValName' },
				note: { fi: 'tValNote' },
				isSum: false
			}
		]
	},
	{
		code: 'msVar',
		name: { fi: 'msVarAName' },
		note: { fi: 'msVarANote' },
		type: VariableType.OtherClassificatory,
		values: [
			{
				code: 'msVal1',
				name: { fi: 'msVal1Name' },
				note: { fi: 'msVal1Note' },
				isSum: false
			},
			{
				code: 'msVal2',
				name: { fi: 'msVal2Name' },
				note: { fi: 'msVal2Note' },
				isSum: false
			}
		]
	}
];

const mockSettingsChangedHandler = jest.fn();
const mockVisualizationSettings: IVisualizationSettings = {
	multiselectableVariableCode: 'msVar'
};
const mockVisualizationRules = {
	allowManualPivot: false, sortingOptions: null, multiselectVariableAllowed: true
};

jest.mock('react-i18next', () => ({
	...jest.requireActual('react-i18next'),
	useTranslation: () => {
		return {
			t: (str: string) => str,
			i18n: {
				changeLanguage: () => new Promise(() => undefined),
			},
		};
	},
}));

describe('Rendering test', () => {
	it('renders correctly', () => {
		const { asFragment } = render(
			<MultiselectableSelector
				visualizationRules={mockVisualizationRules}
				settingsChangedHandler={mockSettingsChangedHandler}
				visualizationSettings={mockVisualizationSettings}
				variables={mockVariables}
			/>);
		expect(asFragment()).toMatchSnapshot();
	});
});



describe('Assertion tests', () => {
	it('When no multiselect variable code is provided, the selector should default to "noMultiselectable"', () => {
		const mockSettingsChangedHandler = jest.fn();
		render(<MultiselectableSelector
			variables={mockVariables}
			visualizationRules={mockVisualizationRules}
			visualizationSettings={{ ...mockVisualizationSettings, multiselectableVariableCode: null }}
			settingsChangedHandler={mockSettingsChangedHandler}
		></MultiselectableSelector>);
		expect(screen.getByDisplayValue("noMultiselectable")).toBeInTheDocument();
	});

	it('When multiselect variable code is provided, the selector should be rendered with a corresponding value', () => {
		const mockSettingsChangedHandler = jest.fn();
		const changedMockSettings = { ...mockVisualizationSettings, multiselectableVariableCode: "msVar" }
		render(<MultiselectableSelector
			variables={mockVariables}
			visualizationRules={mockVisualizationRules}
			visualizationSettings={changedMockSettings}
			settingsChangedHandler={mockSettingsChangedHandler}
		></MultiselectableSelector>);
		expect(screen.queryByDisplayValue("msVar")).toBeInTheDocument();
	});
});