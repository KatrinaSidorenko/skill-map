import { Edge, Node } from '@xyflow/react';
import dagre from '@dagrejs/dagre';
import { v4 as uuidv4 } from 'uuid';

const getNodePosition = (index: number): { x: number; y: number } => ({
  x: 0,
  y: index * 150, // space nodes 150px apart vertically
});

export const generateId = () => uuidv4().replaceAll('-', '');

const nodeWidth = 180;
const nodeHeight = 100;

export function getLayoutedElements(
  nodes: Node[],
  edges: Edge[],
  direction: 'TB' | 'LR' = 'LR',
) {
  const dagreGraph = new dagre.graphlib.Graph();
  dagreGraph.setDefaultEdgeLabel(() => ({}));

  dagreGraph.setGraph({ rankdir: direction });

  nodes.forEach((node) => {
    dagreGraph.setNode(node.id, { width: nodeWidth, height: nodeHeight });
  });

  edges.forEach((edge) => {
    dagreGraph.setEdge(edge.source, edge.target);
  });

  dagre.layout(dagreGraph);

  const layoutedNodes = nodes.map((node) => {
    const nodeWithPosition = dagreGraph.node(node.id);
    node.position = {
      x: nodeWithPosition.x - nodeWidth / 2,
      y: nodeWithPosition.y - nodeHeight / 2,
    };
    return node;
  });

  return { nodes: layoutedNodes, edges };
}

export function mapRoadmapToReactFlow(roadmap: Roadmap): {
  nodes: Node[];
  edges: Edge[];
} {
  const nodes: Node[] = roadmap.nodes.map((n, index) => ({
    id: String(n.id),
    position: getNodePosition(index),
    data: {
      label: n.title,
      description: n.description,
    },
    type: 'default',
  }));

  const edges: Edge[] = roadmap.edges.map((e) => ({
    id: `${e.source}-${e.target}`,
    source: String(e.source),
    target: String(e.target),
  }));

  return getLayoutedElements(nodes, edges, 'TB');
}

export function mapRoadmapToReactFlowForSaved(roadmap: SavedRoadmap): {
  nodes: Node[];
  edges: Edge[];
} {
  const nodes: Node[] = roadmap.nodes.map((n, index) => ({
    id: String(n.id),
    position: getNodePosition(index),
    data: {
      label: n.title,
      description: n.description,
      status: n.status,
    },
    type: 'statusNode',
  }));

  const edges: Edge[] = roadmap.edges.map((e) => ({
    id: `${e.source}-${e.target}`,
    source: String(e.source),
    target: String(e.target),
  }));

  return getLayoutedElements(nodes, edges, 'TB');
}

export const defaultPagination = {
  pageSize: 12,
  pageNumber: 1,
};

export const getStatusColor = (status: LearningStatus) => {
  switch (status) {
    case 'completed':
      return 'green';
    case 'inprogress':
      return 'blue';
    case 'notstarted':
    default:
      return 'gray';
  }
};

export const getProgressInPercentage = (progress: number) => {
  return Math.round(progress * 100);
};
