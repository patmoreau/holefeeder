export const widgetsDirectory = 'file:///group/widgets';

export const createWidget = jest.fn(() => ({
  updateTimeline: jest.fn(),
  updateSnapshot: jest.fn(),
  getTimeline: jest.fn(),
  reload: jest.fn(),
}));
