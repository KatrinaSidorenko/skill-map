import { Edge, Node } from '@xyflow/react';

const getNodePosition = (index: number): { x: number; y: number } => ({
  x: 0,
  y: index * 150, // space nodes 150px apart vertically
});

export function mapRoadmapToReactFlow(roadmap: Roadmap): {
  nodes: Node[];
  edges: Edge[];
} {
  const nodes: Node[] = roadmap.nodes.map((n, index) => ({
    id: String(n.id),
    position: getNodePosition(index),
    data: { label: n.title }, // show title in node
  }));

  const edges: Edge[] = roadmap.edges.map((e) => ({
    id: `${e.source}-${e.target}`,
    source: String(e.source),
    target: String(e.target),
  }));

  return { nodes, edges };
}

export const defaultPagination = {
  pageSize: 6,
  pageNumber: 1,
};
